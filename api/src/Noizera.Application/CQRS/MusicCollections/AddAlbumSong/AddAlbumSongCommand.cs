using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Songs;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.AddAlbumSong;

public sealed record AddAlbumSongCommand(
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<AddAlbumSongResponse>
{
    public sealed class Handler(
        IAlbumRepository albumRepository,
        IUserRepository userRepository,
        ISongRepository songRepository)
        : IRequestHandler<AddAlbumSongCommand, AddAlbumSongResponse>
    {
        public async Task<AddAlbumSongResponse> Handle([NotNull] AddAlbumSongCommand request, CancellationToken cancellationToken)
        {
            var album = await albumRepository.GetFullAsync(request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            var user = await userRepository.GetWithSongsAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("User not found", ErrorType.NotFound);

            Song song = Song.Create(user, album);
            await songRepository.InsertAsync(song, cancellationToken).ConfigureAwait(false);

            return new(song.Id, song.PublicId);
        }
    }
}
