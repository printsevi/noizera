using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.AddAlbumSong;

public sealed record AddAlbumSongCommand(
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<AddAlbumSongResponse>
{
    public sealed class Handler(
        IHashGenerator hashGenerator,
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

            var song = await Song.NewAsync(user, album, hashGenerator, cancellationToken).ConfigureAwait(false);
            await songRepository.InsertAsync(song, cancellationToken).ConfigureAwait(false);

            return new(song.Id, song.PublicId);
        }
    }
}
