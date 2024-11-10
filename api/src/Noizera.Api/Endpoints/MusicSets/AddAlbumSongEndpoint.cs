using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.AddAlbumSong;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class AddAlbumSongEndpoint : IEndpoint
{
    internal sealed record Request(Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/music-collections/albums/{albumId}/songs", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid albumId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddAlbumSongCommand command = new(albumId, request.UserId);

        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}