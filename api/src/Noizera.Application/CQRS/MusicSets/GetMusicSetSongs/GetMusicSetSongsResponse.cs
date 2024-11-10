using Noizera.Shared.Contracts.QueryResults;

namespace Noizera.Application.CQRS.MusicSets.GetMusicSetSongs;

public sealed record GetMusicSetSongsResponse(
    IEnumerable<MusicSetSongResult> Songs);
