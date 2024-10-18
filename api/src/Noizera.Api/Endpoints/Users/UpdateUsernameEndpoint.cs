using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users.UpdateUsername;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UpdateUsernameEndpoint : IEndpoint
{
    internal sealed record Request(string NewUsername);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/username", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateUsernameCommand command = new(request.NewUsername, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}