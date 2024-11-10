using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.UpdateMusicSetTitle;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class UpdateMusicSetTitleEndpoint : IEndpoint
{
    private sealed record Request(string NewTitle, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/{MusicSetId}/title", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid MusicSetId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateMusicSetTitleCommand command = new(MusicSetId, request.NewTitle, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}