using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Application.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.MusicSets;

public sealed record AddNewAlbumCreditCommand(Guid AlbumId, string CreditProfileName, Guid UserId) : IAuthorizeableRequest<IdResponse>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<AddNewAlbumCreditCommand, IdResponse>
    {
        public async Task<IdResponse> Handle([NotNull] AddNewAlbumCreditCommand request, CancellationToken cancellationToken)
        {
            var album = await db.Albums.FirstOrDefaultAsync(x => x.Id == request.AlbumId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"An Album {request.AlbumId} not found", ErrorType.NotFound);

            AlbumCredit result = AlbumCredit.New(album, ProfileType.Artist, request.CreditProfileName);

            await db.InsertAsync(result, cancellationToken).ConfigureAwait(false);

            return new(result.Id);
        }
    }
}

public sealed class AddNewAlbumCreditValidator : AbstractValidator<AddNewAlbumCreditCommand>
{
    public AddNewAlbumCreditValidator()
    {
        _ = RuleFor(x => x.CreditProfileName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
        _ = RuleFor(x => x.AlbumId).NotEmpty();
    }
}
