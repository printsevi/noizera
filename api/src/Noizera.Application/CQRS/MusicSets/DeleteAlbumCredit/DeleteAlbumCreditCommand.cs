using MediatR;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.DeleteAlbumCredit;

public sealed record DeleteAlbumCreditCommand(Guid CreditId, Guid UserId) : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        /*IMusicSetCreditRepository MusicSetCreditRepository*/)
        : IRequestHandler<DeleteAlbumCreditCommand, Unit>
    {
        public Unit Handle([NotNull] DeleteAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            //await MusicSetCreditRepository.DeleteCreditAsync(request.CreditId, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }

        Task<Unit> IRequestHandler<DeleteAlbumCreditCommand, Unit>.Handle(DeleteAlbumCreditCommand request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
