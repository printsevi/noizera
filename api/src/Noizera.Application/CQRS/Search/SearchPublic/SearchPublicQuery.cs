using MediatR;
using Noizera.Application.CQRS.Feed.GetFeedPublicCategories;

namespace Noizera.Application.CQRS.Search.SearchPublic;

public sealed record SearchPublicQuery(string searchQuery)
    : IRequest<GetFeedPublicCategoriesResponse>
{
    public sealed class Handler()
        : IRequestHandler<SearchPublicQuery, GetFeedPublicCategoriesResponse>
    {
        public async Task<GetFeedPublicCategoriesResponse> Handle(SearchPublicQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult<GetFeedPublicCategoriesResponse>(
                new([new("new-releases", "New releases")],
                []));
        }
    }
}
