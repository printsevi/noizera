using MediatR;
using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.DeleteAlbumCoverImage;

public sealed record DeleteAlbumCoverImageCommand(
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAlbumRepository albumRepository)
        : IRequestHandler<DeleteAlbumCoverImageCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteAlbumCoverImageCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            album.DeleteCover(request.UserId);

            await albumRepository.UpdateAsync(album, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
