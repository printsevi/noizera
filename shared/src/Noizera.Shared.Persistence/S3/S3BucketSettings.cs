namespace Noizera.Shared.Persistence.S3;

public class S3BucketSettings
{
    public required string OriginalAudio { get; set; }
    public required string FlacAudio { get; set; }
    public required string Mp3Audio { get; set; }
    public required string CoverImages { get; set; }
    public required string ProfileImages { get; set; }
    public required string EmailTemplates { get; set; }
}
