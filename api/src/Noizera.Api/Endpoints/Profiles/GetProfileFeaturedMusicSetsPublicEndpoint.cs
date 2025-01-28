using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetProfileFeaturedMusicSetsPublicEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/public/profiles/{profileUsername}/featured-music-sets", Handle)
            .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string profileUsername,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetProfileFeaturedMusicSetsPublicQuery query = new(profileUsername);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}