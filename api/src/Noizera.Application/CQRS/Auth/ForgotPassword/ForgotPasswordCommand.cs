using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.SecretTokens;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(
    string EmailOrUsername)
    : IRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        ISecretTokenGenerator tokenGenerator,
        ISecretTokenRepository secretTokenRepository,
        IUserRepository userRepository)
        : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var resetToken = SecretToken.NewResetToken(tokenGenerator, user);

            await secretTokenRepository.InsertAsync(resetToken, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
