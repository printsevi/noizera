namespace Noizera.Shared.Infrastructure.Audio;

public class AudioSettings
{
    public required IReadOnlyCollection<string> ValidInputExtensions { get; set; }
    public required string OutputExtension { get; set; }
    public required int OutputSampleRate { get; set; }
    public required int TargetLufsInNegative { get; set; }
    public required int TruePeakInNegative { get; set; }
    public required int BitDepth { get; set; }
}
