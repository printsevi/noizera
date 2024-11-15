using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.Songs;

public sealed record OwnerSongAmountRule(User Owner) : ISyncDomainRule
{
    public string ErrorMessage => $"Amount of songs achieved the limit of {Owner.SongLimitToUpload}.";

    public bool Verify() => Owner.SongLimitToUpload > Owner.Songs.Count;
}
