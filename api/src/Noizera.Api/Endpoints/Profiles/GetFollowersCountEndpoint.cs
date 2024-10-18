using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles.GetFollowersCount;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetFollowersCountEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/profiles/{profilePublicId}/followers-count", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string profilePublicId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFollowersCountQuery query = new(profilePublicId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}