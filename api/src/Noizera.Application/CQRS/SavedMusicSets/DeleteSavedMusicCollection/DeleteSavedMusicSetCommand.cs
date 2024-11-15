using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets.DeleteSavedMusicCollection;

public sealed record DeleteSavedMusicSetCommand(
    string MusicSetPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository,
        ISavedMusicSetRepository savedMusicSetRepository)
        : IRequestHandler<DeleteSavedMusicSetCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteSavedMusicSetCommand request, CancellationToken cancellationToken)
        {
            var MusicSet = await MusicSetRepository.GetAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            await savedMusicSetRepository.DeleteAsync(MusicSet.Id, request.UserId, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
