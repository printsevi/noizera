using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.ProfileRelations;

public class ProfileRelation : BaseEntity
{
    public Guid FollowerProfileId { get; private set; }
    public PublicProfile FollowerProfile { get; } = null!;
    public Guid FollowingProfileId { get; private set; }
    public PublicProfile FollowingProfile { get; } = null!;

    private ProfileRelation(
        Guid followerProfileId,
        Guid followingProfileId)
    {
        FollowerProfileId = followerProfileId;
        FollowingProfileId = followingProfileId;
    }

    public static ProfileRelation New([NotNull] PublicProfile follower, [NotNull] PublicProfile following) 
        => new(follower.Id, following.Id);

    private ProfileRelation() { }
}
