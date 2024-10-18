using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Domain.MusicCollectionCredits;

public sealed class MusicCollectionCredit : Entity
{
    public MusicSet MusicCollection { get; } = null!;
    public Guid MusicCollectionId { get; private init; }
    public ProfileType? ProfileType { get; private init; }
    public string? ProfileName { get; private init; }
    public PublicProfile? Profile { get; }
    public Guid? ProfileId { get; private init; }

    private MusicCollectionCredit(Guid musicCollectionId, Guid profileId) : this(musicCollectionId) => ProfileId = profileId;

    private MusicCollectionCredit(Guid musicCollectionId, ProfileType profileType, string? profileName = null) : this(musicCollectionId)
    {
        ProfileType = profileType;
        ProfileName = profileName;
    }

    private MusicCollectionCredit(Guid musicCollectionId) : base() => MusicCollectionId = musicCollectionId;

    public static MusicCollectionCredit Create(Guid musicCollectionId, ProfileType profileType, string? name = null)
        => new(musicCollectionId, profileType, name);

    public static MusicCollectionCredit Create(Guid musicCollectionId, Guid profileId)
        => new(musicCollectionId, profileId);

    private MusicCollectionCredit() { }
}
