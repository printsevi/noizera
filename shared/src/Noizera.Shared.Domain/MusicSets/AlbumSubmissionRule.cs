using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Profiles;

namespace Noizera.Common.Domain.MusicSets;

public sealed record AlbumSubmissionRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"Submission failed. Missing properties.";

    public bool Verify() =>
        Album.AlbumStatus == AlbumStatus.Draft
        && !string.IsNullOrWhiteSpace(Album.Title)
        && Album.AlbumReleaseDate is not null
        && Album.CoverImageContentLength > 0
        && Album.MusicSetSongs.Count > 0
        && (Album.Owner.Profile.ProfileType == ProfileType.Artist 
            || Album.AlbumCredits.Any(x => x.ProfileType == ProfileType.Artist)
        && Album.MusicSetSongs.All(x =>
            !string.IsNullOrWhiteSpace(x.Song.OriginalFileName)
            && !string.IsNullOrWhiteSpace(x.Song.Title)));
}
