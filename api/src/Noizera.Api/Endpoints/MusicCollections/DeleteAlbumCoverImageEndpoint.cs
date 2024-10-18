using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.DeleteAlbumCoverImage;

namespace Noizera.Api.Endpoints.MusicCollections;

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