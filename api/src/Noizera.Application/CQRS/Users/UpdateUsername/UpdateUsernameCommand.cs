using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.UpdateUsername;

public sealed record UpdateUsernameCommand(
    string NewUsername,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IProfileUniquenessChecker profileUniquenessChecker)
        : IRequestHandler<UpdateUsernameCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateUsernameCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            await user.UpdateUsernameAsync(request.NewUsername, profileUniquenessChecker, cancellationToken).ConfigureAwait(false);

            await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
