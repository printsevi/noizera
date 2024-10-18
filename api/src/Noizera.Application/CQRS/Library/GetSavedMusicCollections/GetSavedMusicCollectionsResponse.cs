using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Application.CQRS.Library.GetSavedMusicCollections;

public sealed record GetSavedMusicCollectionsResponse(List<MusicCollectionCardQueryResult> Collections);
