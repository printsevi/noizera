using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;
using Noizera.Common.Domain.SecretTokens;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.Authenticate;

public sealed record AuthenticateCommand(
    string EmailOrUsername,
    string Code)
    : IRequest<AuthenticateResponse>, ISensitiveRequest
{
    public sealed class Handler(
        ISecretTokenGenerator tokenGenerator,
        IJwtTokenService jwtTokenGenerator,
        ISecretTokenRepository secretTokenRepository,
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository)
        : IRequestHandler<AuthenticateCommand, AuthenticateResponse>
    {
        public async Task<AuthenticateResponse> Handle([NotNull] AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var code = await verificationCodeRepository.GetLatestAsync(user.Email, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"A code for {user.Email} not found", ErrorType.NotFound);

            bool isCodeValid = code.VerifyAndInvalidateCode(request.Code);
            await verificationCodeRepository.UpdateAsync(code, cancellationToken).ConfigureAwait(false);

            if (!isCodeValid)
            {
                throw new AppException($"The code is incorrect", ErrorType.BadRequest);
            }

            string accessToken = jwtTokenGenerator.GenerateAccessToken(user);
            SecretToken refreshToken = SecretToken.NewRefreshToken(tokenGenerator, user);

            await secretTokenRepository.InsertAsync(refreshToken, cancellationToken).ConfigureAwait(false);

            return new(accessToken, refreshToken.Token);
        }
    }
}
