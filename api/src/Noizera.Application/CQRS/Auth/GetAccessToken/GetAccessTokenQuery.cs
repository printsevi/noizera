using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;
using Noizera.Common.Domain.SecretTokens;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Auth.GetAccessToken;

public sealed record GetAccessTokenQuery(
    string RefreshToken,
    string ExpiredAccessToken)
    : IRequest<GetAccessTokenResponse>, ISensitiveRequest
{
    public sealed class Handler(
        ISecretTokenGenerator tokenGenerator,
        IJwtTokenService jwtTokenService,
        ISecretTokenRepository secretTokenRepository,
        IUserRepository userRepository)
        : IRequestHandler<GetAccessTokenQuery, GetAccessTokenResponse>
    {
        public async Task<GetAccessTokenResponse> Handle([NotNull] GetAccessTokenQuery request, CancellationToken cancellationToken)
        {
            var userId = jwtTokenService.GetUserId(request.ExpiredAccessToken)
                ?? throw new AppException("JWT token is incorrect", ErrorType.BadRequest);

            var user = await userRepository.GetWithActiveRefreshTokensAsync(userId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var refreshToken = user.GetTokenOfValue(request.RefreshToken)
                ?? throw new AppException("Your session is expired. Please sign in.", ErrorType.Authorization, ErrorCode.RefreshTokenRevoked);

            refreshToken.RevokeToken();

            await secretTokenRepository.UpdateAsync(refreshToken, cancellationToken).ConfigureAwait(false);

            string accessToken = jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = SecretToken.NewRefreshToken(tokenGenerator, user);

            await secretTokenRepository.InsertAsync(newRefreshToken, cancellationToken).ConfigureAwait(false);

            return new(accessToken, newRefreshToken.Token);
        }
    }
}
