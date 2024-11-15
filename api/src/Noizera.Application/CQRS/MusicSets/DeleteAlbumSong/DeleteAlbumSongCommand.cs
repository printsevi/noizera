using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Contracts.Services;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.DeleteAlbumSong;

public sealed record DeleteAlbumSongCommand(
    Guid AlbumId,
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAlbumRepository albumRepository,
        ISongRepository songRepository,
        IAudioFileService audioService)
        : IRequestHandler<DeleteAlbumSongCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteAlbumSongCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetFullAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            album.ValidateOwner(request.UserId);
            album.ValidateDraft();

            var collectionSong = album.MusicSetSongs.FirstOrDefault(x => x.SongId == request.SongId)
                ?? throw new AppException("Song is not found", ErrorType.NotFound);

            await audioService.DeleteOriginalAudioFileAsync(collectionSong.Song.PublicId, cancellationToken).ConfigureAwait(false);
            await songRepository.DeleteAsync(request.SongId, cancellationToken).ConfigureAwait(false);

            album = await albumRepository.GetFullAsync(request.AlbumId, cancellationToken).ConfigureAwait(false);
            album!.FixSongSequences();
            await albumRepository.UpdateAsync(album, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
