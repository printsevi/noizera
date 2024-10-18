namespace Noizera.Application.CQRS.Feed.GetFeedCategories;

public record GetFeedCategoriesResponse(
    ICollection<GetFeedMusicCollectionItem> MusicCategories,
    ICollection<GetFeedProfileItem> ProfileCategories);

public record GetFeedMusicCollectionItem(
    string Api,
    string Title
);

public record GetFeedProfileItem(
    string Api,
    string Title
);
