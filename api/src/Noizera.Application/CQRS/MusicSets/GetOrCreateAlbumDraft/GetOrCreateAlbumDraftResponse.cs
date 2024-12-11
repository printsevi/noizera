namespace Noizera.Application.CQRS.MusicSets.GetOrCreateAlbumDraft;

public sealed record GetOrCreateAlbumDraftResponse(
    Guid AlbumId,
    string AlbumPublicId,
    string Title,
    DateOnly? ReleaseDate,
    string? CoverImageS3Folder,
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
    long? ContentLength,
    string? ContentType,
    short Sequence);

public sealed record GetOrCreateAlbumDraftCreditResponse(
    Guid Id,
    string? ProfileUsername,
    string? ProfilePublicId,
    Guid? ProfileId,
    string ProfileName);
