using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionSongSequence;

public sealed record UpdateMusicCollectionSongSequenceCommand(
    Guid MusicCollectionId,
    Guid ActiveSongId,
    Guid OverSongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<UpdateMusicCollectionSongSequenceCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateMusicCollectionSongSequenceCommand request, CancellationToken cancellationToken)
        {
            var musicCollection = await musicCollectionRepository.GetWithSongsAsync(request.MusicCollectionId, cancellationToken).ConfigureAwait(false);
            if (musicCollection is null)
            {
                throw new AppException("Music Collection not found", ErrorType.NotFound);
            }

            musicCollection.SwitchSongs(request.ActiveSongId, request.OverSongId, request.UserId);

            await musicCollectionRepository.UpdateAsync(musicCollection, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
