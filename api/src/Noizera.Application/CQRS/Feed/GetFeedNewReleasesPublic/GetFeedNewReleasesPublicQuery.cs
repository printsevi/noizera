using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleasesPublic;

public sealed record GetFeedNewReleasesPublicQuery()
    : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db,
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedNewReleasesPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle(GetFeedNewReleasesPublicQuery request, CancellationToken cancellationToken)
        {
            var newReleases = await MusicSetRepository.GetNewReleasesAsync(cancellationToken).ConfigureAwait(false);
            var albumPublicIds = newReleases.Select(x => x.PublicId).Distinct();
            var credits = await db.GetAlbumsCreditsAsync(albumPublicIds, cancellationToken).ConfigureAwait(false);
            var result = newReleases.Select(x => new MusicSetCardQueryResult(
                x.PublicId,
                x.Title,
                x.CollectionType,
                x.ReleaseDate,
                x.IsSaved,
                x.OwnerUsername,
                x.OwnerName,
                x.OwnerProfileType,
                x.SongCount,
                credits.Where(c => c.MusicSetPublicId == x.PublicId)
            ));
            
            return result.ToList();
        }
    }
}
