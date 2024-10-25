using MediatR;
using Microsoft.AspNetCore.Mvc;
using Noizera.Api.Common;
using Noizera.Application.CQRS.MusicCollections.UpdateAlbumReleaseDate;
using Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionTitle;

namespace Noizera.Api.Endpoints.MusicCollections;

internal sealed class UpdateAlbumReleaseDateEndpoint : IEndpoint
{
    private sealed record Request(string? NewDate);

    public void Setup(IEndpointRouteBuilder app)
        => app.MapPut("/api/music-collections/albums/{albumId}/release-date", Handle)
              .RequireAuthorization();

    private static async Task<IResult> Handle(
        Guid albumId,
        Guid userId,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        UpdateAlbumReleaseDateCommand command = new(albumId, request.NewDate, userId);

        _ = await sender.Send(command, ct).ConfigureAwait(false);

        return Results.Ok();
    }
}