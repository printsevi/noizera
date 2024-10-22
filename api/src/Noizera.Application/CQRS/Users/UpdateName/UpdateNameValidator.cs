using FluentValidation;

namespace Noizera.Application.CQRS.Users.UpdateName;

public sealed class UpdateNameValidator : AbstractValidator<UpdateNameCommand>
{
    public UpdateNameValidator()
    {
        _ = RuleFor(x => x.NewName).MaximumLength(30);
    }
}
