using MediatR;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetFollowers;

public sealed record GetFollowersQuery(
    string ProfilePublicId,
    Guid UserId)
    : IAuthorizeableRequest<GetFollowersResponse>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetFollowersQuery, GetFollowersResponse>
    {
        public async Task<GetFollowersResponse> Handle([NotNull] GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.GetFollowersAsync(request.ProfilePublicId, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
