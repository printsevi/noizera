using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users.UpdateProfileType;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UpdateProfileTypeEndpoint : IEndpoint
{
    internal sealed record Request(string NewProfileType);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/profile-type", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateProfileTypeCommand command = new(request.NewProfileType, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}