using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noizera.Infrastructure.Security.Tokens;

public sealed class JwtBearerTokenValidationConfiguration(IOptions<JwtSettings> jwtSettings)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings jwtSettings = jwtSettings.Value;

    public void Configure(string? name, JwtBearerOptions options) => Configure(options);

    public void Configure([NotNull] JwtBearerOptions options) => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.FromMinutes(5),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
    };
}
