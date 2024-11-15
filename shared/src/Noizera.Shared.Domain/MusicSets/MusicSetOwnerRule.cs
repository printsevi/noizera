using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.MusicSets;

public sealed record MusicSetOwnerRule(Guid UserId, MusicSet MusicSet) : ISyncDomainRule
{
    public string ErrorMessage => $"Music collection can be modified only by the music collection's owner.";

    public bool Verify() => MusicSet.OwnerId == UserId;
}
