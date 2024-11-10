using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.SubmitAlbum;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class SubmitAlbumEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPatch("/api/music-collections/albums/{albumId}", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid albumId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SubmitAlbumCommand command = new(albumId, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}