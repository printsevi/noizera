using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.GetMusicCollection;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class GetMusicCollectionEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{collectionPublicId}", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        Guid? userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicCollectionQuery query = new(collectionPublicId, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}