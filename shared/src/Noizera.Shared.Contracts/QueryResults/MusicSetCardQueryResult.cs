namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetCardQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    bool IsSaved,
    string OwnerPublicId,
    string OwnerName,
    short SongCount);
