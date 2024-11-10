using MediatR;
using Microsoft.AspNetCore.Http;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.MusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.UploadAlbumCoverImage;

public sealed record UploadAlbumCoverImageCommand(
    IFormFile File,
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        ICoverImageUploader uploader,
        IAlbumRepository albumRepository)
        : IRequestHandler<UploadAlbumCoverImageCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UploadAlbumCoverImageCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            await album.UploadCoverAsync(request.UserId, uploader, request.File, cancellationToken).ConfigureAwait(false);

            await albumRepository.UpdateAsync(album, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
