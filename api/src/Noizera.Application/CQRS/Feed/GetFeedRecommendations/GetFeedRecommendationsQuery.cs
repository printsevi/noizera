using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Feed.GetFeedRecommendations;

public sealed record GetFeedRecommendationsQuery(Guid UserId)
    : IAuthorizeableRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetFeedRecommendationsQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle([NotNull] GetFeedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetRecommendationsAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            
            return result;
        }
    }
}
