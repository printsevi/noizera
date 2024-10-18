using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.UploadAudioFile;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class UploadAudioFileEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/songs/{songId}/audio", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        HttpContext context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        context.Features.Get<IHttpMaxRequestBodySizeFeature>()!.MaxRequestBodySize = 300 * 1024 * 1024; // 300 MB
        var form = await context.Request.ReadFormAsync(ct).ConfigureAwait(false);
        var file = form.Files[0];
        UploadAudioFileCommand command = new(file, songId, userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}