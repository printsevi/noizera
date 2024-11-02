using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.SavedMusicCollections.AddSavedMusicCollection;
using Noizera.Application.CQRS.SavedMusicCollections.DeleteSavedMusicCollection;

namespace Noizera.Api.Endpoints.SavedMusicCollections;

internal sealed class DeleteSavedMusicCollectionEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/saved-collections/{musicCollectionPublicId}", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        string musicCollectionPublicId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteSavedMusicCollectionCommand command = new(musicCollectionPublicId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}