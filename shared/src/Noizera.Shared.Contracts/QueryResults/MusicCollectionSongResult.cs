namespace Noizera.Shared.Contracts.QueryResults;

public record MusicCollectionSongResult(
    string SongPublicId,
    string Title,
    long ContentLength,
    double DurationInSeconds,
    short Sequence);
