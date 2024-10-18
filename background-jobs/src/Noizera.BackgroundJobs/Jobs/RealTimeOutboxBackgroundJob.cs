namespace Noizera.BackgroundJobs.Jobs;

public class RealTimeOutboxBackgroundJob(
    IServiceScopeFactory factory)
    : OutboxBackgroundJob<RealTimeOutboxBackgroundJob>(factory)
{
    public override bool IsRealTime => true;
    public override short MaxBunchAmount { get; set; } = 100;
}
