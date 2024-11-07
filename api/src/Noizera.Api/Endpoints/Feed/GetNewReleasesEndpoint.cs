using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Feed.GetFeedNewReleases;

namespace Noizera.Api.Endpoints.Feed;

internal sealed class GetNewReleasesEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/feed/collections/new-releases", Handle)
            .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFeedNewReleasesQuery query = new(userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}