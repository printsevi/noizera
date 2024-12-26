using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class GetAudioPresignedUrlEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/songs/{songPublicId}/audio-url", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string songPublicId,
        Guid userId,
        string audioType,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetAudioPresignedUrlQuery command = new(songPublicId, audioType, userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}