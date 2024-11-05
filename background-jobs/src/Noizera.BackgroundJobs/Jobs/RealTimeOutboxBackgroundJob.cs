using Microsoft.Extensions.Options;
using Noizera.BackgroundJobs.Common;

namespace Noizera.BackgroundJobs.Jobs;

public class RealTimeOutboxBackgroundJob(
    IServiceScopeFactory factory,
    IOptions<EventSettings> eventSettings)
    : OutboxBackgroundJob<RealTimeOutboxBackgroundJob>(factory, eventSettings)
{
    public override bool IsRealTime => true;
    public override short MaxBunchAmount { get; set; } = 100;
}
