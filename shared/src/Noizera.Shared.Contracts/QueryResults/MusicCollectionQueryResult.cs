namespace Noizera.Shared.Contracts.QueryResults;

public record MusicCollectionQueryResult(
    string PublicId,
    string Title,
    string CollectionType,
    bool IsFavourite);
