using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleases;

public sealed record GetFeedNewReleasesQuery(Guid UserId)
    : IAuthorizeableRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetFeedNewReleasesQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle([NotNull] GetFeedNewReleasesQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetNewReleasesAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
