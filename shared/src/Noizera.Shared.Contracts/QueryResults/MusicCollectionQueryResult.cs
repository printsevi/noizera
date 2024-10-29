namespace Noizera.Shared.Contracts.QueryResults;

public record MusicCollectionQueryResult(
    string Title,
    string CollectionType,
    //bool? IsFavourite,
    string? Description,
    DateOnly? ReleaseDate);
