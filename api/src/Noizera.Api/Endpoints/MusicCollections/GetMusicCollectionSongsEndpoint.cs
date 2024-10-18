using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.GetMusicCollectionSongs;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class GetMusicCollectionSongsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{collectionPublicId}/songs", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicCollectionSongsQuery query = new(collectionPublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}