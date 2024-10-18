using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Subscriptions.GetSubscriptions;

namespace Noizera.Api.Endpoints.Subscriptions;

internal sealed class GetSubscriptionsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/subscriptions", Handle)
              .AllowAnonymous();

    internal static async Task<IResult> Handle(
        string? profileType,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetSubscriptionsQuery command = new(profileType);
        var response = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(response);
    }
}