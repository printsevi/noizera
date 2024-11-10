using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.UpdateMusicSetSongSequence;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class UpdateMusicSetSongSequenceEndpoint : IEndpoint
{
    private sealed record Request(Guid ActiveSongId, Guid OverSongId, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/{MusicSetId}/songs/sequences", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid MusicSetId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateMusicSetSongSequenceCommand command = new(MusicSetId, request.ActiveSongId, request.OverSongId, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}