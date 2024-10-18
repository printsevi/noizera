using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record AlbumSubmissionRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"Submission failed. Missing properties.";

    public bool Verify() =>
        Album.AlbumStatus == AlbumStatus.Draft
        && !string.IsNullOrWhiteSpace(Album.Title)
        //&& Album.ReleaseDate is not null
        && Album.MusicCollectionSongs.Count > 0
        && !Album.MusicCollectionSongs.Any(x => string.IsNullOrWhiteSpace(x.Song.OriginalFileName) || string.IsNullOrWhiteSpace(x.Song.Title))
        && !string.IsNullOrWhiteSpace(Album.CoverImageMongoId)
        && (Album.Owner.Profile!.ProfileType == ProfileType.Artist
            || Album.Credits.Any(x => x.ProfileType == ProfileType.Artist));
}
