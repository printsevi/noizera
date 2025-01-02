namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    DateOnly? ReleaseDate,
    string OwnerUsername,
    string OwnerName,
    string OwnerProfileType,
    short SongCount,
    bool IsSaved);
