namespace Noizera.Infrastructure.Security.Tokens;

public class SecretTokensSettings
{
    public const string Section = "SecretTokens";

    public required int RefreshTokenExpirationInHours { get; set; }
    public required int ResetTokenExpirationInMinutes { get; set; }
}