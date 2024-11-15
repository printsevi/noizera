using MediatR;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.AddSkip;

public sealed record AddSkipCommand(
    Guid SongId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IListeningHistoryRepository listeningHistoryRepository)
        : IRequestHandler<AddSkipCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSkipCommand request, CancellationToken cancellationToken)
        {
            var history = await listeningHistoryRepository.GetAsync(request.UserId, request.SongId).ConfigureAwait(false);
            if (history is not null)
            {
                history.AddSkip();
                await listeningHistoryRepository.UpdateAsync(history, cancellationToken).ConfigureAwait(false);
            }

            return Unit.Value;
        }
    }
}
