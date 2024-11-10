using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicSets.GetOrCreateAlbumDraft;

namespace Noizera.Api.Endpoints.MusicSets;

internal sealed class GetAlbumDraftEndpoint : IEndpoint
{
    public void Setup(IEndpointRouteBuilder app)
        => app.MapGet("/api/music-collections/albums/draft", Handle)
              .RequireAuthorization();

    internal static async Task<IResult> Handle(
        Guid userId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        GetOrCreateAlbumDraftCommand command = new(userId);
        var result = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok(result);
    }
}