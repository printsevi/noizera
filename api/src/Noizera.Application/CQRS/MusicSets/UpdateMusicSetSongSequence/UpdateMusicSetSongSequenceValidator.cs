using FluentValidation;

namespace Noizera.Application.CQRS.MusicSets.UpdateMusicSetSongSequence;

public sealed class UpdateMusicSetSongSequenceValidator : AbstractValidator<UpdateMusicSetSongSequenceCommand>
{
    public UpdateMusicSetSongSequenceValidator()
    {
        _ = RuleFor(x => x.ActiveSongId).NotEmpty();
        _ = RuleFor(x => x.OverSongId).NotEmpty();
    }
}
