using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record DraftAlbumRule(Album Album) : ISyncDomainRule
{
    public string ErrorMessage => $"The action can be applied only for a draft album.";

    public bool Verify() => Album.AlbumStatus is AlbumStatus.Draft;
}
