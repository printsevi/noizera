using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.AddAlbumSong;

public sealed record AddAlbumSongCommand(
    Guid AlbumId,
    Guid UserId)
    : IAuthorizeableRequest<AddAlbumSongResponse>
{
    public sealed class Handler(
        AppDbContext db,
        IHashGenerator hashGenerator)
        : IRequestHandler<AddAlbumSongCommand, AddAlbumSongResponse>
    {
        public async Task<AddAlbumSongResponse> Handle([NotNull] AddAlbumSongCommand request, CancellationToken cancellationToken)
        {
            var album = await db.Albums
                .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                    .ThenInclude(i => i.Song)
                .Include(x => x.Owner)
                    .ThenInclude(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Id == request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Album not found", ErrorType.NotFound);

            var user = await db.Users
                .Include(u => u.Profile)
                .Include(u => u.Songs)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("User not found", ErrorType.NotFound);

            var song = await Song.NewAsync(user, album, hashGenerator, cancellationToken).ConfigureAwait(false);
            await db.InsertAsync(song, cancellationToken).ConfigureAwait(false);

            return new(song.Id, song.PublicId);
        }
    }
}
