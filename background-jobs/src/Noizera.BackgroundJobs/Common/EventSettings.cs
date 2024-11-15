namespace Noizera.BackgroundJobs.Common;

internal class EventSettings
{
    public required bool HandleAllEvents { get; set; }
    public required List<string> EventsToHandle { get; set; }
}
