namespace Noizera.Infrastructure.Security.Tokens;

public class JwtSettings
{
    public const string Section = "JwtSettings";

    public required string Audience { get; set; } = null!;
    public required string Issuer { get; set; } = null!;
    public required string Secret { get; set; } = null!;
    public required int TokenExpirationInMinutes { get; set; }
}