using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.SavedMusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicCollections.AddSavedMusicCollection;

public sealed record AddSavedMusicCollectionCommand(
    string MusicCollectionPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IMusicCollectionRepository musicCollectionRepository,
        ISavedMusicCollectionRepository savedMusicCollectionRepository)
        : IRequestHandler<AddSavedMusicCollectionCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSavedMusicCollectionCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var musicSet = await musicCollectionRepository.GetAsync(request.MusicCollectionPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            var savedMusicCollection = SavedMusicSet.New(user, musicSet);

            await savedMusicCollectionRepository.InsertAsync(savedMusicCollection, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
