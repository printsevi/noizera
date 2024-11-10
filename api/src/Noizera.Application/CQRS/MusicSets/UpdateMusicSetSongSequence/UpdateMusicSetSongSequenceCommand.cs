using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.UpdateMusicSetSongSequence;

public sealed record UpdateMusicSetSongSequenceCommand(
    Guid MusicSetId,
    Guid ActiveSongId,
    Guid OverSongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<UpdateMusicSetSongSequenceCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateMusicSetSongSequenceCommand request, CancellationToken cancellationToken)
        {
            var MusicSet = await MusicSetRepository.GetWithSongsAsync(request.MusicSetId, cancellationToken).ConfigureAwait(false);
            if (MusicSet is null)
            {
                throw new AppException("Music Collection not found", ErrorType.NotFound);
            }

            MusicSet.SwitchSongs(request.ActiveSongId, request.OverSongId, request.UserId);

            await MusicSetRepository.UpdateAsync(MusicSet, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
