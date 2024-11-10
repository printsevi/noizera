using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.DeleteAlbumCoverImage;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class DeleteAlbumCoverImageEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/music-collections/albums/{albumId}/cover-image", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid albumId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteAlbumCoverImageCommand command = new(albumId, userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}