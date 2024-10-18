namespace Noizera.Shared.Contracts.QueryResults;

public record MusicCollectionSongResult(
    string SongPublicId,
    string Title,
    short Sequence);
