using Microsoft.AspNetCore.Antiforgery;
using Noizera.Api.Common;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class GetAntiforgeryTokenEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/auth/antiforgery-token", Handle)
              .RequireAuthorization();

    internal static IResult Handle(
        IAntiforgery forgeryService,
        HttpContext context)
    {
        var tokens = forgeryService.GetAndStoreTokens(context);

        return Results.Ok(tokens.RequestToken);
    }
}