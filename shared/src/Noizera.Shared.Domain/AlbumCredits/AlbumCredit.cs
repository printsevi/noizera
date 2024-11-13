using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Domain.AlbumCredits;

public sealed class AlbumCredit : Entity
{
    public Guid AlbumId { get; }
    public ProfileType? ProfileType { get; }
    public string? ProfileName { get; }
    public Guid? ProfileId { get; }

    public PublicProfile? Profile { get; }
    public Album Album { get; } = null!;

    private AlbumCredit(Album album, ProfileType profileType, string? profileName = null) : this(album)
    {
        ProfileType = profileType;
        ProfileName = profileName;
    }

    private AlbumCredit(Album album) : base()
    {
        AlbumId = album.Id;
    }

    public static AlbumCredit New(Album album, ProfileType profileType, string? name = null)
        => new(album, profileType, name);

    private AlbumCredit() { }
}
