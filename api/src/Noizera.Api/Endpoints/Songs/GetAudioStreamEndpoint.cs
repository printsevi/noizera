using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.GetAudioStream;
using System.Net;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class GetAudioStreamEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/songs/{songPublicId}/audio", Handle)
              .AllowAnonymous();

    private static async Task Handle(
        HttpContext context,
        string songPublicId,
        string audioType,
        long contentLength,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        string requestedRange = context.Request.Headers["Range"].ToString();

        GetAudioStreamQuery query = new(songPublicId, requestedRange, contentLength, audioType);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
        context.Response.Headers["Cache-Control"] = "no-cache";
        context.Response.Headers["Accept-Ranges"] = "bytes";
        context.Response.Headers["Content-Range"] = $"bytes {result.Start}-{result.End}/{result.ContentLength}";
        context.Response.Headers["Content-Length"] = contentLength.ToString();
        context.Response.Headers["Content-Type"] = audioType;

        await result.Stream.CopyToAsync(context.Response.Body, ct).ConfigureAwait(false);
    }
}