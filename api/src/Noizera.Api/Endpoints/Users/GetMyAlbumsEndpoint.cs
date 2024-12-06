using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Users;

namespace Noizera.Api.Endpoints.Users;

internal sealed class GetMyAlbumsEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/users/{userId}/albums", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetMyAlbumsQuery query = new(userId);
        var result = await sender.Send(query, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}