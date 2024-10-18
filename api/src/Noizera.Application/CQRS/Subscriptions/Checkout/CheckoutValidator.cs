using FluentValidation;

namespace Noizera.Application.CQRS.Subscriptions.Checkout;

public sealed class CheckoutValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutValidator()
    {
        _ = RuleFor(x => x.UserId).NotEmpty();
        _ = RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}
