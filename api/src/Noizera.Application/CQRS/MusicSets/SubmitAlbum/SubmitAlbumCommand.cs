using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.SubmitAlbum;

public sealed record SubmitAlbumCommand(
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAlbumRepository albumRepository)
        : IRequestHandler<SubmitAlbumCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] SubmitAlbumCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetFullAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Album {request.AlbumId} not found", ErrorType.NotFound);

            album.Submit(request.UserId);

            await albumRepository.UpdateAsync(album, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
