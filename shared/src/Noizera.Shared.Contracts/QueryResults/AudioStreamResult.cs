namespace Noizera.Shared.Contracts.QueryResults;

public sealed record AudioStreamResult(
    Stream Stream,
    long ContentLength,
    string ContentType,
    long PartLength,
    long Start,
    long End);
