using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollection;

public sealed record GetMusicCollectionResponse(
    string Title);
