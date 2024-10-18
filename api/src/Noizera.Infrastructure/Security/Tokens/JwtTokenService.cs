using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Noizera.Infrastructure.Security.Tokens;

public class JwtTokenService(IOptions<JwtSettings> jwtOptions) : IJwtTokenService
{
    private readonly JwtSettings jwtSettings = jwtOptions.Value;

    public string GenerateAccessToken(User user)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimType.UserId, user.Id.ToString()),
        ];

        foreach (string role in user.SplitRoles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        JwtSecurityToken token = new(
            jwtSettings.Issuer,
            jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid? GetUserId(string expiredAccessToken)
    {
        var principal = GetTokenPrincipal(expiredAccessToken);

        return Guid.TryParse(principal?.Claims.FirstOrDefault(c => c.Type == ClaimType.UserId)?.Value, out var result) ? result : null;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5404:Do not disable token validation checks", Justification = "<Pending>")]
    private ClaimsPrincipal? GetTokenPrincipal(string accessToken)
    {
        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        };

        return new JwtSecurityTokenHandler().ValidateToken(accessToken, validationParameters, out _);
    }
}