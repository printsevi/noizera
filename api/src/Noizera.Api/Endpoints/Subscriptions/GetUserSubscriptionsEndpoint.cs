using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Subscriptions.GetUserSubscriptions;

namespace Noizera.Api.Endpoints.Subscriptions;

internal sealed class GetUserSubscriptionsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/user-subscriptions", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetUserSubscriptionsQuery command = new(userId);
        var response = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(response);
    }
}