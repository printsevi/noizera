using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Feed.GetFeedRecommendations;

namespace Noizera.Api.Endpoints.Feed;

internal sealed class GetFeedRecommendationsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/feed/collections/recommendations", Handle)
            .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFeedRecommendationsQuery query = new(userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}