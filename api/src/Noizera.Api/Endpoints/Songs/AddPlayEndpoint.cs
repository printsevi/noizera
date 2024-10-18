using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.AddPlay;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class AddPlayEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/songs/{songPublicId}/play", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string songPublicId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddPlayCommand command = new(songPublicId, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}