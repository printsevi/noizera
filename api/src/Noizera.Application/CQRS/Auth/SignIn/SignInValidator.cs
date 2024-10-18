using FluentValidation;

namespace Noizera.Application.CQRS.Auth.SignIn;

public sealed class SignInValidator : AbstractValidator<SignInCommand>
{
    public SignInValidator() => RuleFor(x => x.EmailOrUsername).MinimumLength(1);
}
