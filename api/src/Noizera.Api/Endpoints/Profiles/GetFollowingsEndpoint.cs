using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles.GetFollowers;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class GetFollowingsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/profiles/{profilePublicId}/followings", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string profilePublicId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetFollowersQuery query = new(profilePublicId, userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}