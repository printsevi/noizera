using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles.GetArtists;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetArtistsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/profiles/artists", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string text,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetArtistsQuery query = new(text, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}