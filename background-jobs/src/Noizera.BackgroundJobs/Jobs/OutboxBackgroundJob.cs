using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Outbox;
using Noizera.Shared.Persistence.SQL;
using Serilog;

namespace Noizera.BackgroundJobs.Jobs;

public abstract class OutboxBackgroundJob<T>(
    IServiceScopeFactory factory,
    IOptions<EventSettings> eventSettings)
    : BackgroundService where T : BackgroundService
{
    private readonly EventSettings settings = eventSettings.Value;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private int executionCount = 0;
    private int failedCount = 0;

    public bool IsEnabled { get; set; } = true;
    public virtual short MaxBunchAmount { get; set; } = 5;
    public abstract bool IsRealTime { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);
        while (
            !stoppingToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(stoppingToken)
            && IsEnabled
            && failedCount < 1000)
        {
            try
            {
                await using var asyncScope = factory.CreateAsyncScope();
                var mediator = asyncScope.ServiceProvider.GetRequiredService<IMediator>();
                var db = asyncScope.ServiceProvider.GetRequiredService<AppDbContext>();

                var messages = await db.OutboxMessages
                    .AsTracking()
                    .Where(OutboxMessage.IsProcessableExpression(IsRealTime))
                    .Where(x => settings.HandleAllEvents || settings.EventsToHandle.Contains(x.Type))
                    .Take(MaxBunchAmount)
                    .ToListAsync();

                if (!messages.Any())
                {
                    Log.Logger.Information("No outbox message is found");
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                foreach (var message in messages)
                {
                    try
                    {
                        var domainEvent = OutboxConverter.ConvertToDomainEvent(message);
                        var genericDispatcherType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                        if (Activator.CreateInstance(genericDispatcherType, domainEvent) is not INotification notification)
                        {
                            throw new Exception($"{genericDispatcherType.FullName} is not INotification");
                        }

                        await mediator.Publish(notification, stoppingToken);

                        message.Process();
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error("Message: {messageId} is failed: {errorMessage}", message.Id, ex.Message);
                        message.Fail(ex.Message);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);

                executionCount++;

                Log.Logger.Information("{typeName} processed {messagesCount} messages - Count: {executionCount}", typeof(T).Name, messages.Count, executionCount);
            }
            catch (Exception ex)
            {
                failedCount++;
                Log.Logger.Error("Failed to process {typeName} with exception message {errorMessage}", typeof(T).Name, ex.Message);
            }
        }
    }
}
