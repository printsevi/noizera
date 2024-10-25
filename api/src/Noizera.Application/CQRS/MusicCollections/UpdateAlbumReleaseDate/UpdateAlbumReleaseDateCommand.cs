using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.UpdateAlbumReleaseDate;

public sealed record UpdateAlbumReleaseDateCommand(
    Guid AlbumId,
    string? NewDate,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAlbumRepository albumRepository)
        : IRequestHandler<UpdateAlbumReleaseDateCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateAlbumReleaseDateCommand request, CancellationToken cancellationToken)
        {
            DateOnly releaseDate = default;
            if (!string.IsNullOrEmpty(request.NewDate) && !DateOnly.TryParse(request.NewDate, out releaseDate))
            {
                throw new AppException($"Date {request.NewDate} has incorrect format", ErrorType.BadRequest);
            }

            var album = await albumRepository.GetAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            album.SetReleaseDate(!string.IsNullOrEmpty(request.NewDate) ? releaseDate : null);

            await albumRepository.UpdateAsync(album, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
