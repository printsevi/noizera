using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.SavedMusicSets;

public sealed class SavedMusicSet : Entity
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

    public static SavedMusicSet New([NotNull] User user, [NotNull] MusicSet musicSet)
    {
        EnsureRule(new SavedMusicSetRule(user, musicSet));
        EnsureRule(new MusicSetOwnerRule(user, musicSet));

        return new(user, musicSet);
    }

    private SavedMusicSet() { }
}
