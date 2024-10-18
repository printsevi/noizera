using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollectionSongs;

public sealed record GetMusicCollectionSongsResponse(
    IEnumerable<MusicCollectionSongResult> Songs);
