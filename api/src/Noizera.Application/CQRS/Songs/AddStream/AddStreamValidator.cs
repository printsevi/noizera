using FluentValidation;

namespace Noizera.Application.CQRS.Songs.AddStream;

public sealed class AddStreamValidator : AbstractValidator<AddStreamCommand>
{
    public AddStreamValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.SongPublicId).NotEmpty();
        _ = RuleFor(x => x.ListeningTimeInSeconds).GreaterThan(0);
    }
}
