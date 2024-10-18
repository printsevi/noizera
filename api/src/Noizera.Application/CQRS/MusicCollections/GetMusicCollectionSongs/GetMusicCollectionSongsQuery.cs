using MediatR;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.GetMusicCollectionSongs;

public sealed record GetMusicCollectionSongsQuery(
    string MusicCollectionPublicId)
    : IRequest<GetMusicCollectionSongsResponse>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<GetMusicCollectionSongsQuery, GetMusicCollectionSongsResponse>
    {
        public async Task<GetMusicCollectionSongsResponse> Handle([NotNull] GetMusicCollectionSongsQuery request, CancellationToken cancellationToken)
        {
            var songs = await musicCollectionRepository.GetSongsByCollectionPublicIdAsync(request.MusicCollectionPublicId, cancellationToken).ConfigureAwait(false);
            return new(songs);
        }
    }
}
