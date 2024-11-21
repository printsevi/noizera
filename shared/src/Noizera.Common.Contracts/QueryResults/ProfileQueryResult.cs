namespace Noizera.Common.Contracts.QueryResults;

public record ProfileQueryResult(
    string PublicId,
    string Name,
    //bool? IsFollowing,
    string? Bio,
    int FollowersCount,
    int FollowingsCount
    );
