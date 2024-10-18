using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetFollowingsCount;

public sealed record GetFollowingsCountQuery(
    string ProfilePublicId,
    Guid UserId)
    : IAuthorizeableRequest<CountQueryResult>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetFollowingsCountQuery, CountQueryResult>
    {
        public async Task<CountQueryResult> Handle([NotNull] GetFollowingsCountQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.GetFollowingsCountAsync(request.ProfilePublicId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
