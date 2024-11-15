using MediatR;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Feed.GetFeedNewReleases;

public sealed record GetFeedNewReleasesQuery(Guid UserId)
    : IAuthorizeableRequest<List<MusicSetCardQueryResult>>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetFeedNewReleasesQuery, List<MusicSetCardQueryResult>>
    {
        public async Task<List<MusicSetCardQueryResult>> Handle([NotNull] GetFeedNewReleasesQuery request, CancellationToken cancellationToken)
        {
            var result = await MusicSetRepository.GetNewReleasesAsync(request.UserId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
