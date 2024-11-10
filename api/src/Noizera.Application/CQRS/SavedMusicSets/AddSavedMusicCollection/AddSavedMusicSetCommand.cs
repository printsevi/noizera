using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.SavedMusicSets;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets.AddSavedMusicCollection;

public sealed record AddSavedMusicSetCommand(
    string MusicSetPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository,
        IMusicSetRepository MusicSetRepository,
        ISavedMusicSetRepository savedMusicSetRepository)
        : IRequestHandler<AddSavedMusicSetCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSavedMusicSetCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var MusicSet = await MusicSetRepository.GetAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            var savedMusicSet = SavedMusicSet.New(user, MusicSet);

            await savedMusicSetRepository.InsertAsync(savedMusicSet, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
