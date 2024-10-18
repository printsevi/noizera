using MediatR;
using Noizera.Shared.Contracts.Security;

namespace Noizera.Application.CQRS.Feed.GetFeedCategories;

public sealed record GetFeedCategoriesQuery(Guid UserId)
    : IAuthorizeableRequest<GetFeedCategoriesResponse>
{
    public sealed class Handler()
        : IRequestHandler<GetFeedCategoriesQuery, GetFeedCategoriesResponse>
    {
        public async Task<GetFeedCategoriesResponse> Handle(GetFeedCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult<GetFeedCategoriesResponse>(
                new([new("recommendations", "Recommendations")], 
                []));
        }
    }
}
