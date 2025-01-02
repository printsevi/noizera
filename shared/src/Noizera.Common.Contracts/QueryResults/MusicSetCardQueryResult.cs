namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetCardQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    DateOnly? ReleaseDate,
    bool IsSaved,
    string OwnerUsername,
    string OwnerName,
    string OwnerProfileType,
    short SongCount,
    IEnumerable<AlbumCreditQueryResult> Credits);
