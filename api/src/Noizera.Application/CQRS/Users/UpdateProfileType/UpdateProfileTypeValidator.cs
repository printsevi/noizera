using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateProfileType;

public sealed class UpdateProfileTypeValidator : AbstractValidator<UpdateProfileTypeCommand>
{
    public UpdateProfileTypeValidator()
    {
        _ = RuleFor(x => x.NewProfileType).Must(x => x == "Artist" || x == "Fan" || x == "Label");
    }
}
