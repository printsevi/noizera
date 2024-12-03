using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.VerificationCodes;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.SignIn;

public sealed record SignInCommand(
    string EmailOrUsername,
    string Password)
    : IRequest<Unit>
{
    public sealed class Handler(
        IVerificationCodeGenerator verificationCodeGenerator,
        IVerificationCodeRepository verificationCodeRepository,
        IUserRepository userRepository,
        IPasswordHelper passwordHelper)
        : IRequestHandler<SignInCommand, Unit>, ISensitiveRequest
    {
        public async Task<Unit> Handle([NotNull] SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            if (!user.VerifyPassword(request.Password, passwordHelper))
            {
                throw new AppException("Password is incorrect", ErrorType.BadRequest);
            }

            VerificationCode code = VerificationCode.New(user.Email, user.Profile.Name, verificationCodeGenerator);

            await verificationCodeRepository.InsertAsync(code, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
