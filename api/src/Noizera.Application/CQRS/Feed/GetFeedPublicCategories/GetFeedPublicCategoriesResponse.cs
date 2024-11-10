namespace Noizera.Application.CQRS.Feed.GetFeedPublicCategories;

public record GetFeedPublicCategoriesResponse(
    ICollection<GetPublicFeedMusicSetItem> MusicCategories,
    ICollection<GetPublicFeedProfileItem> ProfileCategories);

public record GetPublicFeedMusicSetItem(
    string Api,
    string Title
);

public record GetPublicFeedProfileItem(
    string Api,
    string Title
);
