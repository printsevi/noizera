using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.AddAlbumCredit;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class AddAlbumCreditEndpoint : IEndpoint
{
    internal sealed record Request(Guid CreditProfileId, Guid UserId);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/music-collections/albums/{albumId}/credits", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid albumId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddAlbumCreditCommand command = new(albumId, request.CreditProfileId, request.UserId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}