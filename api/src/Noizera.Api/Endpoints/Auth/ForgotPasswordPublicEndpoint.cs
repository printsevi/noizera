using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.ForgotPassword;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class ForgotPasswordPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPatch("/api/password", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string emailOrUsername,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        ForgotPasswordCommand command = new(emailOrUsername);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}