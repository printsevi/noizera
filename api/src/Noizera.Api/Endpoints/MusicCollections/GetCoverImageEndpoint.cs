using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.GetCoverImage;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class GetCoverImageEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{musicCollectionPublicId}/cover-image", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        string musicCollectionPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetCoverImageQuery query = new(musicCollectionPublicId);
        var stream = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.File(stream, "image/jpeg", enableRangeProcessing: false);
    }
}