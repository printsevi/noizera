using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.Songs;

public sealed record OwnerSongAmountRule(User Owner) : ISyncDomainRule
{
    public string ErrorMessage => $"Amount of songs achieved the limit of {Owner.Profile?.SongLimitToUpload}.";

    public bool Verify() => Owner.Profile is not null && Owner.Profile.SongLimitToUpload > Owner.Songs.Count;
}
