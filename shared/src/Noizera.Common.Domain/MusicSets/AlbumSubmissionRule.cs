using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Profiles;

namespace Noizera.Common.Domain.MusicSets;

public sealed record AlbumSubmissionRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage { get; private set; } = "Submission failed. Missing properties";

    public bool Verify()
    {
        if (Album.AlbumStatus != AlbumStatus.Draft)
        {
            ErrorMessage = "The album must be a 'Draft'";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Album.Title))
        {
            ErrorMessage = "Please fill in the album's title";
            return false;
        }

        if (!Album.AlbumReleaseDate.HasValue)
        {
            ErrorMessage = "Please fill in the album's release date";
            return false;
        }

        if (!Album.CoverImageContentLength.HasValue)
        {
            ErrorMessage = "Please upload a cover image";
            return false;
        }

        if (Album.MusicSetSongs.Count == 0)
        {
            ErrorMessage = "Please add at least one song";
            return false;
        }

        if (Album.Owner.Profile.ProfileType != ProfileType.Artist
            && !Album.AlbumCredits.Any(x => x.ProfileType == ProfileType.Artist || (x.Profile is not null && x.Profile.ProfileType == ProfileType.Artist)))
        {
            ErrorMessage = "The album must have at least one artist assigned";
            return false;
        }

        if (Album.MusicSetSongs.Any(x =>
            string.IsNullOrWhiteSpace(x.Song.OriginalFileName)
            || string.IsNullOrWhiteSpace(x.Song.Title)))
        {
            ErrorMessage = "Please review the added songs. All must have titles and audio files uploaded.";
            return false;
        }

        return true;
    }
}
