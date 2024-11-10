using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.GetCoverImage;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetCoverImageEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{MusicSetPublicId}/cover-image", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        string MusicSetPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetCoverImageQuery query = new(MusicSetPublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.File(result.Stream, result.ContentType, enableRangeProcessing: false);
    }
}