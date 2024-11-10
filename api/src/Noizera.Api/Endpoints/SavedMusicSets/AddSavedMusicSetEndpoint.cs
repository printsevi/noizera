using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.SavedMusicSets.AddSavedMusicCollection;

namespace Noizera.Api.Endpoints.SavedMusicSets;

internal sealed class AddSavedMusicSetEndpoint : IEndpoint
{
    internal sealed record Request(string MusicSetPublicId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/saved-collections", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddSavedMusicSetCommand command = new(request.MusicSetPublicId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}