using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Application.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets.AddAlbumCredit;

public sealed record AddAlbumCreditCommand(Guid AlbumId, Guid CreditProfileId, Guid UserId) : IAuthorizeableRequest<IdResponse>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<AddAlbumCreditCommand, IdResponse>
    {
        public async Task<IdResponse> Handle([NotNull] AddAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            var profile = await db.Profiles.FirstOrDefaultAsync(x => x.Id == request.CreditProfileId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"A profile {request.CreditProfileId} not found", ErrorType.NotFound);

            var album = await db.Albums.FirstOrDefaultAsync(x => x.Id == request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"An Album {request.AlbumId} not found", ErrorType.NotFound);

            AlbumCredit result = AlbumCredit.New(album, profile);

            await db.InsertAsync(result, cancellationToken).ConfigureAwait(false);

            return new(result.Id);
        }
    }
}
