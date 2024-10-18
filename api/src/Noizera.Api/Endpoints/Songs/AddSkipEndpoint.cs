using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.AddSkip;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class AddSkipEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/songs/{songId}/skip", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddSkipCommand command = new(songId, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}