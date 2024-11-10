using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record AlbumSubmissionRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"Submission failed. Missing properties.";

    public bool Verify() =>
        Album.AlbumStatus == AlbumStatus.Draft
        && !string.IsNullOrWhiteSpace(Album.Title)
        && Album.AlbumReleaseDate is not null
        && Album.CoverImageContentLength > 0
        && Album.MusicSetSongs.Count > 0
        && Album.MusicSetSongs.All(x =>
            !string.IsNullOrWhiteSpace(x.Song.OriginalFileName)
            && !string.IsNullOrWhiteSpace(x.Song.Title)
            && (x.Song.Owner.Profile.ProfileType == ProfileType.Artist
                || x.Song.Credits.Any(s => s.ProfileType == ProfileType.Artist)));
}
