using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Subscriptions.GetSubscriptionPortal;

namespace Noizera.Api.Endpoints.Subscriptions;

internal sealed class GetSubscriptionPortalEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/subscriptions/portal", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetSubscriptionPortalQuery command = new(userId);
        var response = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(response);
    }
}