using System.Diagnostics.CodeAnalysis;
using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record GetMusicSetSongsQuery(
    Guid UserId,
    string MusicSetPublicId,
    string AudioType)
    : IAuthorizeableRequest<List<MusicSetSongQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMusicSetSongsQuery, List<MusicSetSongQueryResult>>
    {
        public async Task<List<MusicSetSongQueryResult>> Handle([NotNull] GetMusicSetSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = request.AudioType == "audio/flac"
                ? await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Flac, request.UserId, cancellationToken).ConfigureAwait(false)
                : await db.GetSongsAsync(request.MusicSetPublicId, Common.AudioType.Mpeg, request.UserId, cancellationToken).ConfigureAwait(false);
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
