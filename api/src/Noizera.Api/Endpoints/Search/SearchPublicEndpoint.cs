using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Search;

namespace Noizera.Api.Endpoints.Search;

internal sealed class SearchPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/search", Handle)
            .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string searchQuery,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        SearchPublicQuery query = new(searchQuery);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}