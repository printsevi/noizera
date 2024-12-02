using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs;

public sealed record RemoveSongFromFavouritesCommand(
    Guid MusicSetSongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<RemoveSongFromFavouritesCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] RemoveSongFromFavouritesCommand request, CancellationToken cancellationToken)
        {
            var favouritesPlaylist = await db.Playlists
                .FirstOrDefaultAsync(x => x.PlaylistTag == PlaylistConstants.FavouritesPlaylistTag && x.OwnerId == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The favourites playlist is not found", ErrorType.NotFound);

            _ = await db.MusicSetSongs
                .Where(x => x.Id == request.MusicSetSongId && x.MusicSetId == favouritesPlaylist.Id)
                .ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

            _ = await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
