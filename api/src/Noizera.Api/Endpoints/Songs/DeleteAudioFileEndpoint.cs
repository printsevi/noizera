using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.DeleteAudioFile;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class DeleteAudioFileEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/songs/{songId}/audio", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteAudioFileCommand command = new(songId, userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}