namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetCardQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    bool IsSaved,
    string OwnerUsername,
    string OwnerName,
    short SongCount);
