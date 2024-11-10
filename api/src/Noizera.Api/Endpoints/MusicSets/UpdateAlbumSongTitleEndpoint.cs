using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.UpdateAlbumSongTitle;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class UpdateAlbumSongTitleEndpoint : IEndpoint
{
    private sealed record Request(string NewTitle, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/albums/{albumId}/songs/{songId}/title", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid albumId,
        Guid songId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateAlbumSongTitleCommand command = new(albumId, songId, request.NewTitle, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}