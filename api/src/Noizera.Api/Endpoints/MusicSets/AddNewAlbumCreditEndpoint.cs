using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class AddNewAlbumCreditEndpoint : IEndpoint
{
    internal sealed record Request(string CreditProfileName);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPost("/api/music-collections/albums/{albumId}/new-credits", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid albumId,
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        AddNewAlbumCreditCommand command = new(albumId, request.CreditProfileName, userId);

        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}