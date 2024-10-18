using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.AddSongToFavourites;

public sealed record AddSongToFavouritesCommand(
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IPlaylistRepository playlistRepository,
        ISongRepository songRepository)
        : IRequestHandler<AddSongToFavouritesCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSongToFavouritesCommand request, CancellationToken cancellationToken)
        {
            var song = await songRepository.GetAsync(request.SongId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The song is not found", ErrorType.NotFound);

            var favouritesPlaylist = await playlistRepository.GetFavouritesAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The fovourites playlist is not found", ErrorType.NotFound);

            favouritesPlaylist.AddSong(song);

            await playlistRepository.UpdateAsync(favouritesPlaylist, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
