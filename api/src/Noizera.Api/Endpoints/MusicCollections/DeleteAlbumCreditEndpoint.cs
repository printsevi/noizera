using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.DeleteAlbumCredit;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class DeleteAlbumCreditEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapDelete("/api/music-collections/credits/{creditId}", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid creditId,
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        DeleteAlbumCreditCommand command = new(creditId, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}