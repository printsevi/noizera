using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Application.CQRS.Profiles.GetArtists;

public sealed record GetArtistsResponse(
    Guid ArtistId,
    string Name,
    string PublicId);
