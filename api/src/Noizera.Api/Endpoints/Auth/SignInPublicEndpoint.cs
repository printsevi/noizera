using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.SignIn;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class SignInPublicEndpoint : IEndpoint
{
    internal sealed record Request(string EmailOrUsername, string Password);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth/sign-in", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SignInCommand command = new(request.EmailOrUsername, request.Password);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}