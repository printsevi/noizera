using FluentValidation;

namespace Noizera.Application.CQRS.Auth.Authenticate;

public sealed class AuthenticateValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateValidator()
    {
        _ = RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
            .Matches(@"^[a-zA-Z0-9\s\p{P}\p{S}]*$")
            .WithMessage("The input must contain only Latin letters, numbers, and valid symbols.");

        _ = RuleFor(x => x.Code).NotEmpty();
    }
}
