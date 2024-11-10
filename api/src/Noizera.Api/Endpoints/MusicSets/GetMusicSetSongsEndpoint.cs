using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.GetMusicSetSongs;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetMusicSetSongsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{collectionPublicId}/songs", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        string audioType,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicSetSongsQuery query = new(collectionPublicId, audioType);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}