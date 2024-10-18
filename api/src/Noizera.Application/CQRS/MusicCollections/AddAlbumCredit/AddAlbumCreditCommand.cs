using MediatR;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.MusicCollectionCredits;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicCollections.AddAlbumCredit;

public sealed record AddAlbumCreditCommand(Guid AlbumId, Guid CreditProfileId, Guid UserId) : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IMusicCollectionCreditRepository musicCollectionCreditRepository)
        : IRequestHandler<AddAlbumCreditCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            MusicCollectionCredit credit = MusicCollectionCredit.Create(request.AlbumId, request.CreditProfileId);
            await musicCollectionCreditRepository.InsertAsync(credit, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
