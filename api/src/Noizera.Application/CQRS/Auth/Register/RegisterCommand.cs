using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string ProfileUserName) 
    : IRequest<Unit>, ISensitiveRequest
{
    public sealed class Handler(
        IUserRepository userRepository,
        IUserUniquenessChecker userUniquenessChecker,
        IProfileUniquenessChecker profileUniquenessChecker,
        IPasswordHelper passwordHelper,
        IVerificationCodeRepository verificationCodeRepository) : IRequestHandler<RegisterCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] RegisterCommand request, CancellationToken cancellationToken)
        {
            var code = await verificationCodeRepository.GetLatestAsync(request.Email, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Verification Code is not verified", ErrorType.NotFound);

            if (!code.Verified)
            {
                throw new AppException($"Email is not verified", ErrorType.BadRequest);
            }

            var user = await User.CreateAsync(
                request.Email,
                request.Password,
                Role.User,
                request.ProfileUserName,
                userUniquenessChecker,
                profileUniquenessChecker,
                passwordHelper,
                cancellationToken).ConfigureAwait(false);

            await userRepository.InsertAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
