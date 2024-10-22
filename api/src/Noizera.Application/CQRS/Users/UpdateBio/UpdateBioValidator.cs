using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateBio;

public sealed class UpdateBioValidator : AbstractValidator<UpdateBioCommand>
{
    public UpdateBioValidator()
    {
        _ = RuleFor(x => x.NewBio).MaximumLength(150);
    }
}
