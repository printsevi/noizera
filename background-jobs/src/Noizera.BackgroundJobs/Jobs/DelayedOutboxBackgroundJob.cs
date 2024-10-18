namespace Noizera.BackgroundJobs.Jobs;

public class DelayedOutboxBackgroundJob(
    IServiceScopeFactory factory)
    : OutboxBackgroundJob<RealTimeOutboxBackgroundJob>(factory)
{
    public override bool IsRealTime => false;
    public override short MaxBunchAmount { get; set; } = 10;
}
