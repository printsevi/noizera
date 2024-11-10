namespace Noizera.Application.CQRS.Feed.GetFeedCategories;

public record GetFeedCategoriesResponse(
    ICollection<GetFeedMusicSetItem> MusicCategories,
    ICollection<GetFeedProfileItem> ProfileCategories);

public record GetFeedMusicSetItem(
    string Api,
    string Title
);

public record GetFeedProfileItem(
    string Api,
    string Title
);
