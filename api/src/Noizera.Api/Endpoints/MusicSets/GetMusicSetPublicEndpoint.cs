using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.GetMusicSetPublic;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetMusicSetPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/music-collections/{collectionPublicId}", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicSetPublicQuery query = new(collectionPublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}