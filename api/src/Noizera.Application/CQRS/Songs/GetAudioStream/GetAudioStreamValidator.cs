using FluentValidation;

namespace Noizera.Application.CQRS.Songs.GetAudioStream;

public sealed class GetAudioStreamValidator : AbstractValidator<GetAudioStreamQuery>
{
    public GetAudioStreamValidator()
    {
        _ = RuleFor(x => x.SongPublicId).NotEmpty();
    }
}
