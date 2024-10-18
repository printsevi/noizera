using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.AddStream;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class AddStreamEndpoint : IEndpoint
{
    internal sealed record Request(int ListeningTimeInSeconds);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/songs/{songPublicId}/stream", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        string songPublicId,
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddStreamCommand command = new(songPublicId, userId, request.ListeningTimeInSeconds);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}