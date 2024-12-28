using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Users;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword)
    : IRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        AppDbContext db,
        IPasswordHelper passwordHelper,
        IUserRepository userRepository)
        : IRequestHandler<ResetPasswordCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailWithLatestResetTokenAsync(request.Email, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var resetToken = user.GetTokenOfValue(request.Token);
            if (resetToken is null || !resetToken.IsValid)
            {
                throw new AppException("Token is revoked or expired", ErrorType.BadRequest);
            }

            resetToken.RevokeToken();

            user.UpdatePassword(request.NewPassword, passwordHelper);

            await db.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
