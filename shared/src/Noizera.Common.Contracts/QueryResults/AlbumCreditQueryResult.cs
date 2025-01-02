namespace Noizera.Common.Contracts.QueryResults;

public record AlbumCreditQueryResult(
    string MusicSetPublicId,
    string? Username,
    string? Name,
    string? ProfileName,
    string? ProfileType);
