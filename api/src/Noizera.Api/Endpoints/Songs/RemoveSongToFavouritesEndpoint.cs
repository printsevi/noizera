using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class RemoveSongToFavouritesEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/favourite-songs/{songId}", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        RemoveSongFromFavouritesCommand command = new(songId, userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}