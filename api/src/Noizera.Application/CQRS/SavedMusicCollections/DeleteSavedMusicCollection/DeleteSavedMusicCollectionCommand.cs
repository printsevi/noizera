using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicCollections.DeleteSavedMusicCollection;

public sealed record DeleteSavedMusicCollectionCommand(
    string MusicCollectionPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository,
        ISavedMusicCollectionRepository savedMusicCollectionRepository)
        : IRequestHandler<DeleteSavedMusicCollectionCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteSavedMusicCollectionCommand request, CancellationToken cancellationToken)
        {
            var musicCollection = await musicCollectionRepository.GetAsync(request.MusicCollectionPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            await savedMusicCollectionRepository.DeleteAsync(musicCollection.Id, request.UserId, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
