using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.DeleteAlbumSong;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class DeleteAlbumSongEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/music-collections/albums/{albumId}/songs/{songId}", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid albumId,
        Guid songId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteAlbumSongCommand command = new(albumId, songId, userId);

        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}