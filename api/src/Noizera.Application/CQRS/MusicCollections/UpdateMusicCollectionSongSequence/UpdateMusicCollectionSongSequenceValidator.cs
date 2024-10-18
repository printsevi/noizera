using FluentValidation;

namespace Noizera.Application.CQRS.MusicCollections.UpdateMusicCollectionSongSequence;

public sealed class UpdateMusicCollectionSongSequenceValidator : AbstractValidator<UpdateMusicCollectionSongSequenceCommand>
{
    public UpdateMusicCollectionSongSequenceValidator()
    {
        _ = RuleFor(x => x.ActiveSongId).NotEmpty();
        _ = RuleFor(x => x.OverSongId).NotEmpty();
    }
}
