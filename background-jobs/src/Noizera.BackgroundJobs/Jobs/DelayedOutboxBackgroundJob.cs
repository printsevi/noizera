using Microsoft.Extensions.Options;
using Noizera.BackgroundJobs.Common;

namespace Noizera.BackgroundJobs.Jobs;

internal sealed class DelayedOutboxBackgroundJob(
    IServiceScopeFactory factory,
    IOptions<EventSettings> eventSettings)
    : OutboxBackgroundJob<RealTimeOutboxBackgroundJob>(factory, eventSettings)
{
    public override bool IsRealTime => false;
    public override short MaxBunchAmount { get; set; } = 10;
}
