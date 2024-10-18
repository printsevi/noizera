using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.Authenticate;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class AuthenticatePublicEndpoint : IEndpoint
{
    internal sealed record Request(string EmailOrUsername, string Code);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AuthenticateCommand command = new(request.EmailOrUsername, request.Code);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}