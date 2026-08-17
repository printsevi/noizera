using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.GetAudioStream;
using Noizera.Common.Contracts.Security;
using System.Globalization;
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
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        string requestedRange = context.Request.Headers["Range"].ToString();

        // Anonymous listeners are served the free rendition; if a bearer token was supplied,
        // authentication middleware has already validated it, so the caller can be identified
        // for entitlement checks. The claim is the only trusted source of identity here.
        Guid? userId = Guid.TryParse(
            context.User.FindFirst(ClaimType.UserId)?.Value,
            CultureInfo.InvariantCulture,
            out var parsedUserId)
                ? parsedUserId
                : null;

        GetAudioStreamQuery query = new(songPublicId, requestedRange, audioType, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        context.Response.Headers["Accept-Ranges"] = "bytes";
        context.Response.Headers["Content-Range"] = $"bytes {result.Start}-{result.End}/{result.ContentLength}";
        context.Response.Headers["Content-Length"] = result.PartLength.ToString(CultureInfo.InvariantCulture);
        context.Response.Headers["Content-Type"] = audioType;

        await result.Stream.CopyToAsync(context.Response.Body, ct).ConfigureAwait(false);
    }
}
