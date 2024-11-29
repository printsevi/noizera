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
        IMusicSetRepository MusicSetRepository,
        AppDbContext db)
        : IRequestHandler<DeleteSavedMusicSetCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteSavedMusicSetCommand request, CancellationToken cancellationToken)
        {
            var musicSet = await MusicSetRepository.GetAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            _ = await db.SavedMusicSets
                .Where(x => x.MusicSetId == musicSet.Id && x.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false);

            return Unit.Value;
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