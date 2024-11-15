using Noizera.Common.Contracts.QueryResults;

namespace Noizera.Application.CQRS.Profiles.GetFollowings;

public sealed record GetFollowingsResponse(List<ProfileRelationQueryResult> Followings);
