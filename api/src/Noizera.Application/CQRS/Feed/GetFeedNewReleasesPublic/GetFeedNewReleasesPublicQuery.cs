using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleasesPublic;

public sealed record GetFeedNewReleasesPublicQuery()
    : IRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedNewReleasesPublicQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle(GetFeedNewReleasesPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetNewReleasesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
