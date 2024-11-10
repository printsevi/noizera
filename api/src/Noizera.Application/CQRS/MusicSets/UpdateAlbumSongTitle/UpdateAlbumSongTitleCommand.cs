using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.UpdateAlbumSongTitle;

public sealed record UpdateAlbumSongTitleCommand(
    Guid AlbumId,
    Guid SongId,
    string NewTitle,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IAlbumRepository albumRepository,
        ISongRepository songRepository)
        : IRequestHandler<UpdateAlbumSongTitleCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateAlbumSongTitleCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetFullAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            var song = (album.MusicSetSongs.FirstOrDefault(x => x.SongId == request.SongId)?.Song)
                ?? throw new AppException("Song is not found", ErrorType.NotFound);

            song.SetTitle(request.NewTitle, album, request.UserId);

            await songRepository.UpdateAsync(song, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
