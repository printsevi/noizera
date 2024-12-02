using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Application.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs;

public sealed record AddSongToFavouritesCommand(
    string SongPublicId,
    Guid UserId)
    : IAuthorizeableRequest<IdResponse>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<AddSongToFavouritesCommand, IdResponse>
    {
        public async Task<IdResponse> Handle([NotNull] AddSongToFavouritesCommand request, CancellationToken cancellationToken)
        {
            var song = await db.Songs.FirstOrDefaultByPublicIdAsync(request.SongPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The song is not found", ErrorType.NotFound);

            var favouritesPlaylist = await db.Playlists
                .Include(x => x.MusicSetSongs.OrderBy(x => x.Sequence))
                    .ThenInclude(i => i.Song)
                .FirstOrDefaultAsync(x => x.PlaylistTag == PlaylistConstants.FavouritesPlaylistTag && x.OwnerId == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The favourites playlist is not found", ErrorType.NotFound);

            var result = favouritesPlaylist.AddSong(song);

            await db.InsertAsync(result, cancellationToken).ConfigureAwait(false);

            return new(result.Id);
        }
    }
}

public sealed class AddSongToFavouritesValidator : AbstractValidator<AddSongToFavouritesCommand>
{
    public AddSongToFavouritesValidator()
    {
        _ = RuleFor(x => x.SongPublicId).NotEmpty();
    }
}
