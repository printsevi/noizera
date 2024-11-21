using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateUsername;

public sealed class UpdateUsernameValidator : AbstractValidator<UpdateUsernameCommand>
{
    public UpdateUsernameValidator()
    {
        _ = RuleFor(x => x.NewUsername)
            .MinimumLength(2)
            .MaximumLength(30)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
    }
}
