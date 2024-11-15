using Noizera.Common.Contracts.QueryResults;

namespace Noizera.Application.CQRS.Profiles.GetFollowers;

public sealed record GetFollowersResponse(List<ProfileRelationQueryResult> Followers);
