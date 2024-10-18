using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles.Unfollow;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class UnfollowEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/profiles/{profileId}/follower", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid profileId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UnfollowCommand query = new(profileId, userId);
        await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}