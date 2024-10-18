namespace Noizera.Application.CQRS.MusicCollections.GetOrCreateAlbumDraft;

public sealed record GetOrCreateAlbumDraftResponse(
    Guid AlbumId,
    string AlbumPublicId,
    string Title,
    string? Description,
    string? CoverImageMongoId,
    string? CoverImageOriginalName,
    string ProfileName,
    string ProfileType,
    IEnumerable<GetOrCreateAlbumDraftSongResponse> Songs,
    IEnumerable<GetOrCreateAlbumDraftCreditResponse> Credits);

public sealed record GetOrCreateAlbumDraftSongResponse(
    Guid Key,
    string? Title,
    string? SongPublicId,
    string? OriginalFileName,
    short Sequence);

public sealed record GetOrCreateAlbumDraftCreditResponse(
    Guid Key,
    string Value);
