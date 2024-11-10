using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.SavedMusicSets;

public sealed class SavedMusicSet : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Guid MusicSetId { get; private set; }
    public MusicSet MusicSet { get; private set; } = null!;

    private SavedMusicSet(User user, MusicSet MusicSet)
    {
        UserId = user.Id;
        MusicSetId = MusicSet.Id;
    }

    public static SavedMusicSet New(User user, MusicSet MusicSet)
    {
        return new(user, MusicSet);
    }

    private SavedMusicSet() { }
}
