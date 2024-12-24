using Amazon.S3.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.Common;
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
        //string requestedRange = context.Request.Headers["Range"].ToString();

        //GetAudioStreamQuery query = new(songPublicId, requestedRange, contentLength, audioType);
        //var result = await sender.Send(query, ct).ConfigureAwait(false);

        //context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
        //context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //context.Response.Headers["Pragma"] = "no-cache";
        //context.Response.Headers["Expires"] = "0";
        //context.Response.Headers["Accept-Ranges"] = "bytes";
        //context.Response.Headers["Content-Range"] = $"bytes {result.Start}-{result.End}/{result.ContentLength}";
        //context.Response.Headers["Content-Length"] = result.PartLength.ToString();
        //context.Response.Headers["Content-Type"] = audioType;

        //await result.Stream.CopyToAsync(context.Response.Body, ct).ConfigureAwait(false);



        var totalSize = contentLength;
        string contentType = "audio/mpeg";

        // Determine byte range
        long start = 0, end = totalSize - 1;
        var rangeHeader = context.Request.Headers["Range"].ToString();
        var range = rangeHeader.Replace("bytes=", "").Split('-');
        start = long.Parse(range[0]);
        if (range.Length > 1 && !string.IsNullOrEmpty(range[1]))
        {
            end = long.Parse(range[1]);
        }

        if (start > end || end >= totalSize)
        {
            context.Response.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
            context.Response.Headers.Add("Content-Range", $"bytes */{totalSize}");
            return;
        }
        context.Response.StatusCode = StatusCodes.Status206PartialContent;

        var length = end - start + 1;

        // Prepare response headers
        context.Response.Headers.Add("Accept-Ranges", "bytes");
        context.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalSize}");
        context.Response.ContentLength = length;
        context.Response.ContentType = contentType;

        GetAudioStreamQuery query = new(songPublicId, rangeHeader, contentLength, audioType);
        var result = await sender.Send(query, ct).ConfigureAwait(false);
        using var responseStream = result.Stream;

        // Stream the S3 object to the response
        await responseStream.CopyToAsync(context.Response.Body, ct).ConfigureAwait(false);
    }
}