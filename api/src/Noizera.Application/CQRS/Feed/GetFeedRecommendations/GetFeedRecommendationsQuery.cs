using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Feed.GetFeedRecommendations;

public sealed record GetFeedRecommendationsQuery(Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedRecommendationsQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetFeedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetRecommendationsAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
