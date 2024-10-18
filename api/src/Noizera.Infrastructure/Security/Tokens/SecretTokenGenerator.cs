using Microsoft.Extensions.Options;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.SecretTokens;
using System.Security.Cryptography;
using System.Text;

namespace Noizera.Infrastructure.Security.Tokens;

public class SecretTokenGenerator(IOptions<SecretTokensSettings> options) : ISecretTokenGenerator
{
    private readonly SecretTokensSettings settings = options.Value;

    public (string Token, DateTimeOffset ExpireAt) GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return (Convert.ToBase64String(randomNumber), SystemClock.UtcNow.AddHours(settings.RefreshTokenExpirationInHours));
    }

    public (string Token, DateTimeOffset ExpireAt) GenerateResetToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var sb = new StringBuilder(randomNumber.Length * 2);
        foreach (byte b in randomNumber)
        {
            sb.AppendFormat("{0:x2}", b); // Convert each byte to a hex string
        }

        return (sb.ToString(), SystemClock.UtcNow.AddMinutes(settings.ResetTokenExpirationInMinutes));
    }
}