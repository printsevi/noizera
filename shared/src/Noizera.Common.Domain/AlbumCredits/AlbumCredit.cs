using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.AlbumCredits;

public sealed class AlbumCredit : Entity
{
    public Guid AlbumId { get; private set; }
    public ProfileType? ProfileType { get; private set; }
    public string? ProfileName { get; private set; }
    public Guid? ProfileId { get; private set; }

    public PublicProfile? Profile { get; }
    public Album Album { get; } = null!;

    private AlbumCredit(Album album, ProfileType profileType, string? profileName = null) : this(album)
    {
        ProfileType = profileType;
        ProfileName = profileName;
    }

    private AlbumCredit(Album album, PublicProfile profile) : this(album)
        => ProfileId = profile.Id;

    private AlbumCredit(Album album) : base()
        => AlbumId = album.Id;

    public static AlbumCredit New(Album album, ProfileType profileType, string? name = null)
        => new(album, profileType, name);

    public static AlbumCredit New(Album album, [NotNull] PublicProfile profile)
        => new(album, profile);

    private AlbumCredit() { }
}
