using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.SignOut;

public sealed record SignOutCommand(Guid UserId) : IAuthorizeableRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<SignOutCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] SignOutCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetWithActiveRefreshTokensAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            user.RevokeRefreshTokens();

            await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
