using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.AddAlbumCredit;

public sealed class AddAlbumCreditValidator : AbstractValidator<AddAlbumCreditCommand>
{
    public AddAlbumCreditValidator()
    {
        _ = RuleFor(x => x.CreditProfileId).NotEmpty();
        _ = RuleFor(x => x.AlbumId).NotEmpty();
    }
}
