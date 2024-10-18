using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Subscriptions.Checkout;

namespace Noizera.Api.Endpoints.Subscriptions;

internal sealed class CheckoutEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/subscriptions/{subscriptionId}/checkout", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid subscriptionId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        CheckoutCommand command = new(subscriptionId, userId);
        var response = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(response);
    }
}