using MediatR;

namespace Noizera.Application.CQRS.Feed.GetFeedPublicCategories;

public sealed record GetFeedPublicCategoriesQuery()
    : IRequest<GetFeedPublicCategoriesResponse>
{
    public sealed class Handler()
        : IRequestHandler<GetFeedPublicCategoriesQuery, GetFeedPublicCategoriesResponse>
    {
        public async Task<GetFeedPublicCategoriesResponse> Handle(GetFeedPublicCategoriesQuery request, CancellationToken cancellationToken)
        {
            var result = await Task.FromResult<GetFeedPublicCategoriesResponse>(
                new([
                    new("popular", "Popular"),
                    new("new-releases", "New releases")],
                [])).ConfigureAwait(false);

            return result;
        }
    }
}
