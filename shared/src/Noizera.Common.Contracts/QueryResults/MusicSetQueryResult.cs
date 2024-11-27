namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetQueryResult(
    string Title,
    string CollectionType,
    string? Description,
    DateOnly? ReleaseDate,
    string OwnerUsername,
    string OwnerName,
    string OwnerProfileType);
