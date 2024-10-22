using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users.UpdateBio;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UpdateBioEndpoint : IEndpoint
{
    internal sealed record Request(string NewBio);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/bio", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateBioCommand command = new(request.NewBio, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}