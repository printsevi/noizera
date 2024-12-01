namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetSongResult(
    string SongPublicId,
    string Title,
    long ContentLength,
    double DurationInSeconds,
    short Sequence,
    string OwnerUsername,
    string OwnerName,
    string AlbumPublicId,
    bool? IsFavourite);
