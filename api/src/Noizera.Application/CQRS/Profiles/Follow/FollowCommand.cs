using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.ProfileRelations;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.Follow;

public sealed record FollowCommand(
    Guid ProfileId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IProfileRelationRepository profileRelationRepository)
        : IRequestHandler<FollowCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] FollowCommand request, CancellationToken cancellationToken)
        {
            var followerUser = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var followingProfile = await profileRepository.GetAsync(request.ProfileId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A profile not found", ErrorType.NotFound);

            var profileRelation = ProfileRelation.New(followerUser.Profile!, followingProfile);

            await profileRelationRepository.InsertAsync(profileRelation, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
