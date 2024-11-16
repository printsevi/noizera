namespace Noizera.Common.Contracts.QueryResults;

public record ProfileQueryResult(
    //string? ProfileImageSrc,
    string Name,
    //bool? IsFollowing,
    string? Bio,
    int FollowersCount,
    int FollowingsCount
    );
