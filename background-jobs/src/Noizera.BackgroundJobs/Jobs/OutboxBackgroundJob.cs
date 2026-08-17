using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Common;
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
    private static readonly TimeSpan maxBackoff = TimeSpan.FromMinutes(5);
    private int executionCount;

    public bool IsEnabled { get; set; } = true;
    public virtual short MaxBunchAmount { get; set; } = 5;
    public abstract bool IsRealTime { get; }

    public int ConsecutiveFailures { get; private set; }

    public DateTimeOffset? LastSuccessfulRunAt { get; private set; }

    public bool IsStopped { get; private set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The pump must survive any handler or infrastructure failure.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(period);

        try
        {
            while (!stoppingToken.IsCancellationRequested
                && await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
            {
                if (!IsEnabled)
                {
                    continue;
                }

                try
                {
                    await ProcessBatchAsync(stoppingToken).ConfigureAwait(false);

                    ConsecutiveFailures = 0;
                    LastSuccessfulRunAt = SystemClock.UtcNow;
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ConsecutiveFailures++;
                    var backoff = ComputeBackoff(ConsecutiveFailures);

                    Log.Logger.Error(
                        "Failed to process {typeName} with exception message {errorMessage}. Consecutive failures: {consecutiveFailures}. Backing off for {backoffSeconds}s.",
                        typeof(T).Name, ex.Message, ConsecutiveFailures, backoff.TotalSeconds);

                    await Task.Delay(backoff, stoppingToken).ConfigureAwait(false);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Host shutdown — the only legitimate way out of this loop.
        }
        finally
        {
            IsStopped = true;
            Log.Logger.Information("{typeName} stopped after {executionCount} passes.", typeof(T).Name, executionCount);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken stoppingToken)
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
            return;
        }

        foreach (var message in messages)
        {
            await ProcessMessageAsync(mediator, message, stoppingToken).ConfigureAwait(false);
        }

        _ = await db.SaveChangesAsync(stoppingToken).ConfigureAwait(false);

        executionCount++;

        Log.Logger.Information("{typeName} processed {messagesCount} messages - Count: {executionCount}", typeof(T).Name, messages.Count, executionCount);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "One poison message must not stall the batch.")]
    private static async Task ProcessMessageAsync(IMediator mediator, OutboxMessage message, CancellationToken stoppingToken)
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
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (RetryException ex)
        {
            message.Postpone(ex.DelayInSeconds);
        }
        catch (Exception ex)
        {
            Log.Logger.Error("Message: {messageId} is failed with error: {errorMessage}. Exception: {errorDetails}", message.Id, ex.Message, ex.ToString());
            message.Fail(ex.Message);
        }
    }

    private static TimeSpan ComputeBackoff(int consecutiveFailures)
    {
        double seconds = 5d * Math.Pow(2, Math.Min(consecutiveFailures - 1, 10));

        return seconds >= maxBackoff.TotalSeconds ? maxBackoff : TimeSpan.FromSeconds(seconds);
    }
}
