using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.GetMusicSet;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetMusicSetEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/{collectionPublicId}", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string collectionPublicId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMusicSetQuery query = new(collectionPublicId, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}