using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Profiles.Unfollow;

public sealed record UnfollowCommand(
    Guid ProfileId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IProfileRelationRepository profileRelationRepository)
        : IRequestHandler<UnfollowCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UnfollowCommand request, CancellationToken cancellationToken)
        {
            var followerUser = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            await profileRelationRepository.DeleteAsync(followerUser.Profile!.Id, request.ProfileId, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
