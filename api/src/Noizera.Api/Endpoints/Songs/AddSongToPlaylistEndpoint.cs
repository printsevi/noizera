using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.AddSongToPlaylist;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class AddSongToPlaylistEndpoint : IEndpoint
{
    internal sealed record Request(Guid PlaylistId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/songs/{songId}/playlist", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddSongToPlaylistCommand command = new(request.PlaylistId, songId, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}