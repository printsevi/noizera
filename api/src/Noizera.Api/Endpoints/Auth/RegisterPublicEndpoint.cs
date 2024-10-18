using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.Register;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class RegisterPublicEndpoint : IEndpoint
{
    internal sealed record Request(string Email, string ProfileUserName, string Password);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth/register", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        RegisterCommand command = new(request.Email, request.Password, request.ProfileUserName);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}