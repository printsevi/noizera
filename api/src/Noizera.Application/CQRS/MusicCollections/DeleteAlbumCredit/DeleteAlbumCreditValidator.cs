using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.DeleteAlbumCredit;

public sealed class DeleteAlbumCreditValidator : AbstractValidator<DeleteAlbumCreditCommand>
{
    public DeleteAlbumCreditValidator() => RuleFor(x => x.CreditId).NotEmpty();
}
