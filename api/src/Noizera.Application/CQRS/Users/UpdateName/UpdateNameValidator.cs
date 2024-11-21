using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateName;

public sealed class UpdateNameValidator : AbstractValidator<UpdateNameCommand>
{
    public UpdateNameValidator()
    {
        _ = RuleFor(x => x.NewName)
            .MinimumLength(0)
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
    }
}
