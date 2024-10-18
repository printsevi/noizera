using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users.CheckUsername;

namespace Noizera.Api.Endpoints.Users;

internal sealed class CheckUsernameEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/users/{userId}/check-username", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        string username,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        CheckUsernameQuery query = new(userId, username);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}