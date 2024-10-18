using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionTitle;

public sealed class UpdateMusicCollectionTitleValidator : AbstractValidator<UpdateMusicCollectionTitleCommand>
{
    public UpdateMusicCollectionTitleValidator()
    {
        _ = RuleFor(x => x.NewTitle).MaximumLength(500);
        _ = RuleFor(x => x.MusicCollectionId).NotEmpty();
    }
}
