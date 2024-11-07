using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;

namespace Noizera.Application.CQRS.Feed.GetFeedPopularPublic;

public sealed record GetFeedPopularPublicQuery()
    : IRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetFeedPopularPublicQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle(GetFeedPopularPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetRecommendationsAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
