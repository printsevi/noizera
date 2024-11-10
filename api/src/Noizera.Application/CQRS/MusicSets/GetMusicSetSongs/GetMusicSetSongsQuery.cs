using MediatR;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.GetMusicSetSongs;

public sealed record GetMusicSetSongsQuery(
    string MusicSetPublicId,
    string AudioType)
    : IRequest<GetMusicSetSongsResponse>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<GetMusicSetSongsQuery, GetMusicSetSongsResponse>
    {
        public async Task<GetMusicSetSongsResponse> Handle([NotNull] GetMusicSetSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = request.AudioType == "audio/flac"
                ? await MusicSetRepository.GetFlacSongsByCollectionPublicIdAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                : await MusicSetRepository.GetMp3SongsByCollectionPublicIdAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false);
            return new(songs);
        }
    }
}
