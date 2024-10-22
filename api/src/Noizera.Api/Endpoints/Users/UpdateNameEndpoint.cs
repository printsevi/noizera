using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users.UpdateName;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UpdateNameEndpoint : IEndpoint
{
    internal sealed record Request(string NewName);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/name", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateNameCommand command = new(request.NewName, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}