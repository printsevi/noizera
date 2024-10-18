using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.GetAccessToken;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class GetAccessTokenPublicEndpoint : IEndpoint
{
    internal sealed record Request(string RefreshToken, string ExpiredAccessToken);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/auth/access-token", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetAccessTokenQuery command = new(request.RefreshToken, request.ExpiredAccessToken);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}