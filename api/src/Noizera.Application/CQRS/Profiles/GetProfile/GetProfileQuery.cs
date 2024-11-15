using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.GetProfile;

public sealed record GetProfileQuery(string ProfilePublicId)
    : IRequest<ProfileQueryResult>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<GetProfileQuery, ProfileQueryResult>
    {
        public async Task<ProfileQueryResult> Handle([NotNull] GetProfileQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.GetProfileAsync(request.ProfilePublicId, cancellationToken)
                ?? throw new AppException($"Profile not found for {request.ProfilePublicId}", ErrorType.NotFound);

            return result;
        }
    }
}
