using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleasesPublic;

public sealed record GetFeedNewReleasesPublicQuery()
    : IRequest<List<MusicCollectionCardQueryResult>>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetFeedNewReleasesPublicQuery, List<MusicCollectionCardQueryResult>>
    {
        public async Task<List<MusicCollectionCardQueryResult>> Handle(GetFeedNewReleasesPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await musicCollectionRepository.GetNewReleasesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
