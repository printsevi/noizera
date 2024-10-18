using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionSongSequence;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class UpdateMusicCollectionSongSequenceEndpoint : IEndpoint
{
    private sealed record Request(Guid ActiveSongId, Guid OverSongId, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/{musicCollectionId}/songs/sequences", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid musicCollectionId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateMusicCollectionSongSequenceCommand command = new(musicCollectionId, request.ActiveSongId, request.OverSongId, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}