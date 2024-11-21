namespace Noizera.Application.CQRS.Profiles.GetArtists;

public sealed record GetArtistsResponse(
    Guid ArtistId,
    string Name,
    string PublicId,
    string Username);
