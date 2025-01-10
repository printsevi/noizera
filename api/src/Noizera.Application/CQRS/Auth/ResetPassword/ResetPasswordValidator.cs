using FluentValidation;

namespace Noizera.Application.CQRS.Auth.ResetPassword;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        _ = RuleFor(x => x.Email).EmailAddress();
        _ = RuleFor(x => x.Token).NotEmpty();
        _ = RuleFor(x => x.NewPassword)
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain one or more capital letters.")
            .Matches("[a-z]").WithMessage("Password must contain one or more lowercase letters.")
            .Matches(@"\d").WithMessage("Password must contain one or more digits.")
            .Matches(@"[~`¿¡!#$%\^&*€£@+÷=\-\[\]\\';,/{}\(\)|\\"":<>\?\.\_]").WithMessage("Password must contain one or more special characters.")
            //.Matches("^[^£# “”]*$").WithMessage("'{PropertyName}' must not contain the following characters £ # “” or spaces.")
            .WithMessage("Password is not strong enough");
    }
}
