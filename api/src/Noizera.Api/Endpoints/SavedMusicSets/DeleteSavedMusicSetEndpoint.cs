using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.SavedMusicSets.DeleteSavedMusicCollection;

namespace Noizera.Api.Endpoints.SavedMusicSets;

internal sealed class DeleteSavedMusicSetEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/saved-collections/{MusicSetPublicId}", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        string MusicSetPublicId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteSavedMusicSetCommand command = new(MusicSetPublicId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}