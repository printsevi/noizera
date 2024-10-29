using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.GetMusicCollectionPublic;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class GetMusicCollectionPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/music-collections/{collectionPublicId}", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicCollectionPublicQuery query = new(collectionPublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}