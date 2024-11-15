using MediatR;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Repositories;

namespace Noizera.Application.CQRS.Search.FastSearchPublic;

public sealed record FastSearchPublicQuery(string SearchQuery)
    : IRequest<List<FastSearchQueryResult>>
{
    public sealed class Handler(
        IProfileRepository profileRepository)
        : IRequestHandler<FastSearchPublicQuery, List<FastSearchQueryResult>>
    {
        public async Task<List<FastSearchQueryResult>> Handle(FastSearchPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await profileRepository.FastSearchAsync(request.SearchQuery, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
