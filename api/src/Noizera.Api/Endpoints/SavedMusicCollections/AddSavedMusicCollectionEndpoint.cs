using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.SavedMusicCollections.AddSavedMusicCollection;

namespace Noizera.Api.Endpoints.SavedMusicCollections;

internal sealed class AddSavedMusicCollectionEndpoint : IEndpoint
{
    internal sealed record Request(string MusicCollectionPublicId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/saved-collections", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddSavedMusicCollectionCommand command = new(request.MusicCollectionPublicId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}