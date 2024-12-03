using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets;

public sealed record DeleteSavedMusicSetCommand(
    string MusicSetPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<DeleteSavedMusicSetCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteSavedMusicSetCommand request, CancellationToken cancellationToken)
        {
            var musicSet = await db.MusicSets.FirstOrDefaultByPublicIdAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            int result = await db.SavedMusicSets
                .Where(x => x.MusicSetId == musicSet.Id && x.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

            return result == 0
                ? throw new AppException("The collection is not found or can't be deleted.", ErrorType.NotFound)
                : Unit.Value;
        }
    }
}

public sealed class DeleteSavedMusicSetValidator : AbstractValidator<DeleteSavedMusicSetCommand>
{
    public DeleteSavedMusicSetValidator()
    {
        _ = RuleFor(x => x.MusicSetPublicId).NotEmpty();
    }
}