using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.MusicSets;

public sealed record MusicSetOwnerRule(Guid UserId, MusicSet MusicSet) : ISyncDomainRule
{
    public string ErrorMessage => $"Music collection can be modified only by the music collection's owner.";

    public bool Verify() => MusicSet.OwnerId == UserId;
}
