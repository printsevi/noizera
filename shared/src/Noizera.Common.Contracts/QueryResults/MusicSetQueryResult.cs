namespace Noizera.Common.Contracts.QueryResults;

public record MusicSetQueryResult(
    string Title,
    string CollectionType,
    DateOnly? ReleaseDate,
    string OwnerUsername,
    string OwnerName,
    string OwnerProfileType,
    bool IsSaved);
