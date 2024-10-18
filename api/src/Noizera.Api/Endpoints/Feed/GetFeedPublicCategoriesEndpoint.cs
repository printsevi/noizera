using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Feed.GetFeedPublicCategories;

namespace Noizera.Api.Endpoints.Feed;

internal sealed class GetFeedPublicCategoriesEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/feed/public-categories", Handle)
            .AllowAnonymous();

    internal static async Task<IResult> Handle(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFeedPublicCategoriesQuery query = new();
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}