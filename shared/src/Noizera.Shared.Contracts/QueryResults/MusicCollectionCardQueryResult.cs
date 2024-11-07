namespace Noizera.Shared.Contracts.QueryResults;

public record MusicCollectionCardQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    bool IsSaved,
    string OwnerPublicId,
    string OwnerName,
    short SongCount);
