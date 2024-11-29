using FluentValidation;
using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.SavedMusicSets;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.SavedMusicSets;

public sealed record AddSavedMusicSetCommand(
    string MusicSetPublicId,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db,
        IUserRepository userRepository,
        IMusicSetRepository MusicSetRepository)
        : IRequestHandler<AddSavedMusicSetCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] AddSavedMusicSetCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A user not found", ErrorType.NotFound);

            var MusicSet = await MusicSetRepository.GetAsync(request.MusicSetPublicId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException("A music collection not found", ErrorType.NotFound);

            SavedMusicSet savedMusicSet = SavedMusicSet.New(user, MusicSet);

            await db.InsertAsync(savedMusicSet, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}

public sealed class AddSavedMusicSetValidator : AbstractValidator<AddSavedMusicSetCommand>
{
    public AddSavedMusicSetValidator()
    {
        _ = RuleFor(x => x.MusicSetPublicId).NotEmpty();
    }
}
