using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Streams;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Songs.AddStream;

public sealed record AddStreamCommand(
    string SongPublicId,
    Guid UserId,
    int ListeningTimeInSeconds)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IListeningHistoryRepository listeningHistoryRepository,
        ISongRepository songRepository,
        IStreamInfoRepository streamInfoRepository)
        : IRequestHandler<AddStreamCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddStreamCommand request, CancellationToken cancellationToken)
        {
            var song = await songRepository.GetAsync(request.SongPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("The song is not found", ErrorType.NotFound);

            var history = await listeningHistoryRepository.GetAsync(request.UserId, song.Id).ConfigureAwait(false);
            if (history is not null)
            {
                history.AddStream(request.ListeningTimeInSeconds);
                await listeningHistoryRepository.UpdateAsync(history, cancellationToken).ConfigureAwait(false);
            }

            var streamInfo = StreamInfo.New(request.UserId, song.Id, request.ListeningTimeInSeconds);
            await streamInfoRepository.InsertAsync(streamInfo, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
