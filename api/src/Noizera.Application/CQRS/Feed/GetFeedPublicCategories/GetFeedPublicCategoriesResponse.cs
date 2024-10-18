namespace Noizera.Application.CQRS.Feed.GetFeedPublicCategories;

public record GetFeedPublicCategoriesResponse(
    ICollection<GetPublicFeedMusicCollectionItem> MusicCategories,
    ICollection<GetPublicFeedProfileItem> ProfileCategories);

public record GetPublicFeedMusicCollectionItem(
    string Api,
    string Title
);

public record GetPublicFeedProfileItem(
    string Api,
    string Title
);
