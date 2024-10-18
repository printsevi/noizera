using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.ResetPassword;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class ResetPasswordPublicEndpoint : IEndpoint
{
    internal sealed record Request(string Email, string Token, string NewPassword);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/password", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        ResetPasswordCommand command = new(request.Email, request.Token, request.NewPassword);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}