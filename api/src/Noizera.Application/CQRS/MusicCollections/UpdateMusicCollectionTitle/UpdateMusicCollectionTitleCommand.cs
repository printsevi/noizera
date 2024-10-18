using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionTitle;

public sealed record UpdateMusicCollectionTitleCommand(
    Guid MusicCollectionId,
    string NewTitle,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<UpdateMusicCollectionTitleCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateMusicCollectionTitleCommand request, CancellationToken cancellationToken)
        {
            var musicCollection = await musicCollectionRepository.GetAsync(request.MusicCollectionId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Music Collection not found", ErrorType.NotFound);

            musicCollection.SetTitle(request.NewTitle, request.UserId);

            await musicCollectionRepository.UpdateAsync(musicCollection, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
