using MediatR;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetFollowings;

public sealed record GetFollowingsQuery(
    string ProfilePublicId,
    Guid UserId)
    : IAuthorizeableRequest<GetFollowingsResponse>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetFollowingsQuery, GetFollowingsResponse>
    {
        public async Task<GetFollowingsResponse> Handle([NotNull] GetFollowingsQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.GetFollowingsAsync(request.ProfilePublicId, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
