using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetProfileMusicSetsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/profiles/{profileUsername}/music-sets", Handle)
            .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string profileUsername,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetProfileMusicSetsQuery query = new(profileUsername, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}