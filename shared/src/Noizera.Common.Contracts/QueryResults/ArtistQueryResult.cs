namespace Noizera.Common.Contracts.QueryResults;

public record ArtistQueryResult(
    Guid ArtistId,
    string Name,
    string PublicId);
