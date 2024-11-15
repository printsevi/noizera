using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.AddSongToPlaylist;

public sealed record AddSongToPlaylistCommand(
    Guid PlaylistId,
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IPlaylistRepository playlistRepository,
        ISongRepository songRepository)
        : IRequestHandler<AddSongToPlaylistCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSongToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.GetAsync(request.PlaylistId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The playlist is not found", ErrorType.NotFound);

            playlist.ValidateOwner(request.UserId);

            var song = await songRepository.GetAsync(request.SongId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The song is not found", ErrorType.NotFound);

            playlist.AddSong(song);

            await playlistRepository.UpdateAsync(playlist, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
