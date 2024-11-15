namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetQueryResult(
    string Title,
    string CollectionType,
    //bool? IsFavourite,
    string? Description,
    DateOnly? ReleaseDate);
