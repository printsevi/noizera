using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetAlbumCreditsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/albums/{albumId}/credits", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string albumId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetAlbumCreditsQuery query = new(albumId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}