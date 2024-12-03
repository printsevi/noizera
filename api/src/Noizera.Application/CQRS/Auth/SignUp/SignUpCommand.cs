using MediatR;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.VerificationCodes;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.SignUp;

public sealed record SignUpCommand(string Email)
    : IRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        IUserUniquenessChecker userUniquenessChecker,
        IVerificationCodeGenerator verificationCodeGenerator,
        IVerificationCodeRepository verificationCodeRepository)
        : IRequestHandler<SignUpCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] SignUpCommand request, CancellationToken cancellationToken)
        {
            await User.VerifyEmailAsync(request.Email, userUniquenessChecker, cancellationToken).ConfigureAwait(false);

            VerificationCode code = VerificationCode.New(request.Email, request.Email, verificationCodeGenerator);

            await verificationCodeRepository.InsertAsync(code, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
