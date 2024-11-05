namespace Noizera.BackgroundJobs.Common;

public class EventSettings
{
    public required bool HandleAllEvents { get; set; }
    public required string[] EventsToHandle { get; set; }
}
