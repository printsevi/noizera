using System.Diagnostics.CodeAnalysis;
using MediatR;
using Noizera.Application.CQRS.Feed.Common;
using Noizera.Common.Contracts.QueryResults;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Application.CQRS.Feed;

public sealed record GetFeedArtistsQuery(Guid UserId)
    : IAuthorizeableRequest<List<ProfileCardQueryResult>>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetFeedArtistsQuery, List<ProfileCardQueryResult>>
    {
        public async Task<List<ProfileCardQueryResult>> Handle([NotNull] GetFeedArtistsQuery request, CancellationToken cancellationToken)
        {
            var result = await db.GetProfilesAsync(ProfileType.Artist, request.UserId, cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
