using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleases;

public sealed record GetFeedNewReleasesQuery()
    : IRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetFeedNewReleasesQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle(GetFeedNewReleasesQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetPublicRecommendationsAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
