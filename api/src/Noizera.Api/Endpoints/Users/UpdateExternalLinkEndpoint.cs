using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users;

namespace Noizera.Api.Endpoints.Users;

internal sealed class UpdateExternalLinkEndpoint : IEndpoint
{
    internal sealed record Request(string ExternalLink);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/users/{userId}/external-link", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateExternalLinkCommand command = new(request.ExternalLink, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}