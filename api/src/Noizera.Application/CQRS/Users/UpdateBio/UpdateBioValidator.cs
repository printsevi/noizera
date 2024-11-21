using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateBio;

public sealed class UpdateBioValidator : AbstractValidator<UpdateBioCommand>
{
    public UpdateBioValidator()
    {
        _ = RuleFor(x => x.NewBio)
            .MaximumLength(150)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
    }
}
