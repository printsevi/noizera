using FluentValidation;

namespace Noizera.Application.CQRS.Auth.SignIn;

public sealed class SignInValidator : AbstractValidator<SignInCommand>
{
    public SignInValidator() => RuleFor(x => x.EmailOrUsername)
        .NotEmpty()
        .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");
}
