using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetFollowersCount;

public sealed record GetFollowersCountQuery(
    string ProfilePublicId)
    : IRequest<CountQueryResult>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetFollowersCountQuery, CountQueryResult>
    {
        public async Task<CountQueryResult> Handle([NotNull] GetFollowersCountQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.GetFollowersCountAsync(request.ProfilePublicId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
