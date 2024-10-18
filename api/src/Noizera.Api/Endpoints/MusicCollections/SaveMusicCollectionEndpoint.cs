using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.SaveMusicCollection;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class SaveMusicCollectionEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/music-collections/{musicCollectionId}/save", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid musicCollectionId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SaveMusicCollectionCommand command = new(musicCollectionId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}