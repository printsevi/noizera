using MediatR;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Application.CQRS.Profiles.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record GetMusicSetSongsPublicQuery(
    string MusicSetPublicId)
    : IRequest<List<MusicSetSongQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMusicSetSongsPublicQuery, List<MusicSetSongQueryResult>>
    {
        public async Task<List<MusicSetSongQueryResult>> Handle([NotNull] GetMusicSetSongsPublicQuery request, CancellationToken cancellationToken)
        {
            var songs = await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Mpeg, null, cancellationToken).ConfigureAwait(false);
            var albumPublicIds = songs.Select(x => x.AlbumPublicId).Distinct();
            var credits = await db.GetAlbumsCreditsAsync(albumPublicIds, cancellationToken).ConfigureAwait(false);
            var result = songs.Select(x => new MusicSetSongQueryResult(
                x.SongPublicId,
                x.Title,
                x.ContentLength,
                x.DurationInSeconds,
                x.Sequence,
                x.OwnerUsername,
                x.OwnerName,
                x.OwnerProfileType,
                x.AlbumPublicId,
                x.FavouriteSongId,
                credits.Where(c => c.MusicSetPublicId == x.AlbumPublicId)
            ));

            return result.ToList();
        }
    }
}
