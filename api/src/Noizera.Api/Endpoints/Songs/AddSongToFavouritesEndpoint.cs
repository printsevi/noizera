using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.Songs.AddSongToFavourites;

namespace Noizera.Api.Endpoints.Songs;

internal sealed class AddSongToFavouritesEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/songs/{songId}/favourites", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid songId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddSongToFavouritesCommand command = new(songId, userId);
        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}