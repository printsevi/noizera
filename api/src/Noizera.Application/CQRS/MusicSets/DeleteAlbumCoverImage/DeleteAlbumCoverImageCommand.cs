using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.DeleteAlbumCoverImage;

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
