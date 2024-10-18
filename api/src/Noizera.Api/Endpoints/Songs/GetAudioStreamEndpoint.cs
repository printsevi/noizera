using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.GetAudioStream;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class GetAudioStreamEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/songs/{songPublicId}/audio", Handle)
              .AllowAnonymous();

    private static async Task<IResult> Handle(
        string songPublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetAudioStreamQuery query = new(songPublicId);
        var stream = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.File(stream, "audio/mp3", enableRangeProcessing: true);
    }
}