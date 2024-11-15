using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.MusicSets;

public sealed record DraftAlbumRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"The action can be applied only for a draft album.";

    public bool Verify() => Album.AlbumStatus is AlbumStatus.Draft;
}
