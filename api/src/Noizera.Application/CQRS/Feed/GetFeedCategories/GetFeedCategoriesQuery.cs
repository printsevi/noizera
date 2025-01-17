using MediatR;
using Noizera.Common.Contracts.Security;

namespace Noizera.Application.CQRS.Feed.GetFeedCategories;

public sealed record GetFeedCategoriesQuery(Guid UserId)
    : IAuthorizeableRequest<GetFeedCategoriesResponse>
{
    public sealed class Handler()
        : IRequestHandler<GetFeedCategoriesQuery, GetFeedCategoriesResponse>
    {
        public async Task<GetFeedCategoriesResponse> Handle(GetFeedCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await Task.FromResult<GetFeedCategoriesResponse>(
                new([new("recommendations", "Recommendations"),
                    new("new-releases", "New Releases")],
                [new("artists", "Our artists"),
                    new("labels", "Our labels")])).ConfigureAwait(false);

            return result;
        }
    }
}
