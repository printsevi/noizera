using MediatR;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
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
