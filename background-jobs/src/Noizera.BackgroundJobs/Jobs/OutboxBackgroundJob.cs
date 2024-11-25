using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Outbox;
using Noizera.Common.Persistence.SQL;
using Serilog;

namespace Noizera.BackgroundJobs.Jobs;

internal abstract class OutboxBackgroundJob<T>(
    IServiceScopeFactory factory,
    IOptions<EventSettings> eventSettings)
    : BackgroundService where T : BackgroundService
{
    private readonly EventSettings settings = eventSettings.Value;
    private readonly TimeSpan period = TimeSpan.FromSeconds(5);
    private int executionCount;
    private int failedCount;

    public bool IsEnabled { get; set; } = true;
    public virtual short MaxBunchAmount { get; set; } = 5;
    public abstract bool IsRealTime { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(period);
        while (
            !stoppingToken.IsCancellationRequested
            && await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false)
            && IsEnabled
            && failedCount < 1000)
        {
            try
            {
                using var asyncScope = factory.CreateAsyncScope();
                var mediator = asyncScope.ServiceProvider.GetRequiredService<IMediator>();
                var db = asyncScope.ServiceProvider.GetRequiredService<AppDbContext>();

                var messages = await db.OutboxMessages
                    .AsTracking()
                    .Where(OutboxMessage.IsProcessableExpression(IsRealTime))
                    .Where(x => settings.HandleAllEvents || settings.EventsToHandle.Contains(x.Type))
                    .Take(MaxBunchAmount)
                    .ToListAsync(stoppingToken).ConfigureAwait(false);

                if (messages.Count == 0)
                {
                    await Task.Delay(5000, stoppingToken).ConfigureAwait(false);
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
                            throw new TypeLoadException($"{genericDispatcherType.FullName} is not INotification");
                        }

                        await mediator.Publish(notification, stoppingToken).ConfigureAwait(false);

                        message.Process();
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error("Message: {messageId} is failed: {errorMessage}", message.Id, ex.Message);
                        message.Fail(ex.Message);
                    }
                }

                _ = await db.SaveChangesAsync(stoppingToken).ConfigureAwait(false);

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
