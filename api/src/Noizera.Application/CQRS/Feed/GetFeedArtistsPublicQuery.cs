using System.Diagnostics.CodeAnalysis;
using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed;

public sealed record GetFeedArtistsPublicQuery()
    : IRequest<List<ProfileCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetFeedArtistsPublicQuery, List<ProfileCardQueryResult>>
    {
        public async Task<List<ProfileCardQueryResult>> Handle([NotNull] GetFeedArtistsPublicQuery request, CancellationToken cancellationToken)
        {
            var result = await db.GetProfilesAsync(ProfileType.Artist, null, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
