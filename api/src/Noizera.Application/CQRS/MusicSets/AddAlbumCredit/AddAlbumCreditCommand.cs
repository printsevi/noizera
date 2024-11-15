using MediatR;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.AddAlbumCredit;

public sealed record AddAlbumCreditCommand(Guid AlbumId, Guid CreditProfileId, Guid UserId) : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        /*IMusicSetCreditRepository MusicSetCreditRepository*/)
        : IRequestHandler<AddAlbumCreditCommand, Unit>
    {
        public Unit Handle([NotNull] AddAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            //MusicSetCredit credit = SongCredit.Create(request.AlbumId, request.CreditProfileId);
            //await MusicSetCreditRepository.InsertAsync(credit, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }

        Task<Unit> IRequestHandler<AddAlbumCreditCommand, Unit>.Handle(AddAlbumCreditCommand request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
