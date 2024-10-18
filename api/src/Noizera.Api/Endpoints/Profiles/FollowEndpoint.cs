using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Profiles.Follow;

namespace Noizera.Api.Endpoints.Profiles;

internal sealed class FollowEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/profiles/{profileId}/follower", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid profileId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        FollowCommand query = new(profileId, userId);
        await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}