using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.VerifyEmail;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class VerifyEmailPublicEndpoint : IEndpoint
{
    internal sealed record Request(string Email, string Code);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth/verify-email", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        VerifyEmailCommand command = new(request.Email, request.Code);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}