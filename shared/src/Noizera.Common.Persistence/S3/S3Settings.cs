namespace Noizera.Common.Persistence.S3;

public class S3Settings
{
    public required string SpacesKey { get; set; }
    public required string SpacesSecret { get; set; }
    public required Uri ServiceUrl { get; set; }
}
