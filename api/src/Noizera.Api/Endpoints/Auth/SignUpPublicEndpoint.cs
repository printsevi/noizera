using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.SignUp;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class SignUpPublicEndpoint : IEndpoint
{
    internal sealed record Request(string Email);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth/sign-up", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SignUpCommand command = new(request.Email);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}