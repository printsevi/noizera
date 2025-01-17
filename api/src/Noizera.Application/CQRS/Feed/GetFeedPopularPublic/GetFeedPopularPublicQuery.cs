using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Application.CQRS.MusicSets.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed.GetFeedPopularPublic;

public sealed record GetFeedPopularPublicQuery()
    : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db,
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedPopularPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle(GetFeedPopularPublicQuery request, CancellationToken cancellationToken)
        {
            var recommendations = await MusicSetRepository.GetRecommendationsAsync(cancellationToken).ConfigureAwait(false);
            var albumPublicIds = recommendations.Select(x => x.PublicId).Distinct();
            var credits = await db.GetAlbumsCreditsAsync(albumPublicIds, cancellationToken).ConfigureAwait(false);
            var result = recommendations.Select(x => new MusicSetCardQueryResult(
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
