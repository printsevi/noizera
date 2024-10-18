using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Subscriptions.StartSubscription;

namespace Noizera.Api.Endpoints.Subscriptions;

internal sealed class StartSubscriptionEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/subscriptions/checkouts/{checkoutId}/success", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        string checkoutId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        StartSubscriptionCommand command = new(checkoutId, userId);
        var response = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(response);
    }
}