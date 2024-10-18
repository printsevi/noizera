using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Songs;

namespace Noizera.Shared.Domain.SongCredits;

public sealed class SongCredit : Entity
{
    public Song Song { get; } = null!;
    public Guid SongId { get; }
    public ProfileType? ProfileType { get; }
    public string? ProfileName { get; }
    public PublicProfile? Profile { get; }
    public Guid? ProfileId { get; }

    private SongCredit(Song song, ProfileType profileType, string? profileName = null) : this(song)
    {
        ProfileType = profileType;
        ProfileName = profileName;
    }

    private SongCredit(Song song) : base() => SongId = song.Id;

    public static SongCredit Create(Song song, ProfileType profileType, string? name = null)
        => new(song, profileType, name);

    private SongCredit() { }
}
