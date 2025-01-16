namespace Noizera.Common.Contracts.QueryResults;

public record ProfileQueryResult(
    string PublicId,
    string Name,
    string ProfileType,
    //bool? IsFollowing,
    string? Bio,
    int FollowersCount,
    int FollowingsCount,
    string? Link
    );
