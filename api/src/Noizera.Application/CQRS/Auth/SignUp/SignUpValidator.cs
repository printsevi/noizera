using FluentValidation;

namespace Noizera.Application.CQRS.Auth.SignUp;

public sealed class SignUpValidator : AbstractValidator<SignUpCommand>
{
    public SignUpValidator() => RuleFor(x => x.Email).EmailAddress();
}
