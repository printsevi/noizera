using System.Diagnostics.CodeAnalysis;
using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed;

public sealed record GetFeedLabelsPublicQuery()
    : IRequest<List<ProfileCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetFeedLabelsPublicQuery, List<ProfileCardQueryResult>>
    {
        public async Task<List<ProfileCardQueryResult>> Handle([NotNull] GetFeedLabelsPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await db.GetProfilesAsync(ProfileType.Label, null, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
