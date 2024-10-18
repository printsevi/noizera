using MediatR;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.DeleteAlbumCredit;

public sealed record DeleteAlbumCreditCommand(Guid CreditId, Guid UserId) : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicCollectionCreditRepository musicCollectionCreditRepository)
        : IRequestHandler<DeleteAlbumCreditCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            await musicCollectionCreditRepository.DeleteCreditAsync(request.CreditId, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
