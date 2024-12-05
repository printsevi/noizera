using Microsoft.Extensions.Options;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.SecretTokens;
using System.Globalization;
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

        string token = Convert.ToBase64String(randomNumber);

        return (token.Length > Constants.TokenMaxLength ? token[..Constants.TokenMaxLength] : token, SystemClock.UtcNow.AddHours(settings.RefreshTokenExpirationInHours));
    }

    public (string Token, DateTimeOffset ExpireAt) GenerateResetToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        StringBuilder sb = new(randomNumber.Length * 2);
        foreach (byte b in randomNumber)
        {
            _ = sb.AppendFormat(CultureInfo.InvariantCulture, "{0:x2}", b); // Convert each byte to a hex string
        }

        string token = sb.ToString();

        return (token.Length > Constants.TokenMaxLength ? token[..Constants.TokenMaxLength] : token, SystemClock.UtcNow.AddMinutes(settings.ResetTokenExpirationInMinutes));
    }
}