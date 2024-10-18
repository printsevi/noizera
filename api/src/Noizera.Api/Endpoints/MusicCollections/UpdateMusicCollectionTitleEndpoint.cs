using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionTitle;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class UpdateMusicCollectionTitleEndpoint : IEndpoint
{
    private sealed record Request(string NewTitle, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/{musicCollectionId}/title", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid musicCollectionId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateMusicCollectionTitleCommand command = new(musicCollectionId, request.NewTitle, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}