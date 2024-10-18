using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Auth.SignOut;

namespace Noizera.Api.Endpoints.Auth;

internal sealed class SignOutEndpoint : IEndpoint
{
    private sealed record SignOutRequest(Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/auth/sign-out", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        [FromBody] SignOutRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SignOutCommand command = new(request.UserId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}