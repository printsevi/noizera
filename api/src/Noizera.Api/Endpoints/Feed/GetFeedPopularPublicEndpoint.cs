using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Feed.GetFeedPopularPublic;

namespace Noizera.Api.Endpoints.Feed;

internal sealed class GetFeedPopularPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/feed/collections/popular", Handle)
            .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFeedPopularPublicQuery query = new();
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}