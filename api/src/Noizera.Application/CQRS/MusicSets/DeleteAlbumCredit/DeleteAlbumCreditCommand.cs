using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.DeleteAlbumCredit;

public sealed record DeleteAlbumCreditCommand(Guid CreditId, Guid UserId) : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<DeleteAlbumCreditCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            _ = await db.AlbumCredits
                .Where(x => x.Id == request.CreditId)
                .ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
