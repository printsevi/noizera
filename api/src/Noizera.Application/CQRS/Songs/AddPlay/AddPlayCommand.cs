using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.AddPlay;

public sealed record AddPlayCommand(
    string SongPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IListeningHistoryRepository listeningHistoryRepository,
        ISongRepository songRepository)
        : IRequestHandler<AddPlayCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddPlayCommand request, CancellationToken cancellationToken)
        {
            var song = await songRepository.GetAsync(request.SongPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The song is not found", ErrorType.NotFound);
            var history = await listeningHistoryRepository.GetAsync(request.UserId, song.Id).ConfigureAwait(false);
            if (history is not null)
            {
                history.AddPlay();
                await listeningHistoryRepository.UpdateAsync(history, cancellationToken).ConfigureAwait(false);
            }

            return Unit.Value;
        }
    }
}
