using MediatR;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;

namespace Noizera.Application.CQRS.Feed.GetFeedPopularPublic;

public sealed record GetFeedPopularPublicQuery()
    : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedPopularPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle(GetFeedPopularPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetRecommendationsAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
