using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.UpdateMusicSetTitle;

public sealed record UpdateMusicSetTitleCommand(
    Guid MusicSetId,
    string NewTitle,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<UpdateMusicSetTitleCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateMusicSetTitleCommand request, CancellationToken cancellationToken)
        {
            var MusicSet = await MusicSetRepository.GetAsync(request.MusicSetId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("Music Collection not found", ErrorType.NotFound);

            MusicSet.SetTitle(request.NewTitle, request.UserId);

            await MusicSetRepository.UpdateAsync(MusicSet, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
