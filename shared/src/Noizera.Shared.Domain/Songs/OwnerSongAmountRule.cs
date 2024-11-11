using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.Songs;

public sealed record OwnerSongAmountRule(User Owner) : ISyncDomainRule
{
    public string ErrorMessage => $"Amount of songs achieved the limit of {Owner.SongLimitToUpload}.";

    public bool Verify() => Owner.SongLimitToUpload > Owner.Songs.Count;
}
