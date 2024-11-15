namespace Noizera.Common.Domain.SecretTokens;

public interface ISecretTokenGenerator
{
    (string Token, DateTimeOffset ExpireAt) GenerateRefreshToken();

    (string Token, DateTimeOffset ExpireAt) GenerateResetToken();
}