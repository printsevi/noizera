using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.SaveMusicCollection;

public sealed record SaveMusicCollectionCommand(
    Guid MusicCollectionId, 
    Guid UserId) 
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IMusicCollectionRepository musicCollectionRepository)
        : IRequestHandler<SaveMusicCollectionCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] SaveMusicCollectionCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetWithSavedCollectionsAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var musicSet = await musicCollectionRepository.GetAsync(request.MusicCollectionId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            user.SaveMusicCollection(musicSet);

            await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
