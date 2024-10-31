using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Search.FastSearchPublic;

namespace Noizera.Api.Endpoints.Search;

internal sealed class FastSearchPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/fast-search", Handle)
            .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string searchQuery,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        FastSearchPublicQuery query = new(searchQuery);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}