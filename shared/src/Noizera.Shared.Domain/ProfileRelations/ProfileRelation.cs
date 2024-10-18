using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Profiles;

namespace Noizera.Shared.Domain.ProfileRelations;

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

    public static ProfileRelation New(PublicProfile follower, PublicProfile following)
    {
        return new(follower.Id, following.Id);
    }

    private ProfileRelation() { }
}
