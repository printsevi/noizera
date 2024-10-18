using FluentValidation;

namespace Noizera.Application.CQRS.Auth.ResetPassword;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        _ = RuleFor(x => x.Email).EmailAddress();
        _ = RuleFor(x => x.Token).NotEmpty();
        _ = RuleFor(x => x.NewPassword).NotEmpty();
    }
}
