using FluentValidation;

namespace Noizera.Application.CQRS.Subscriptions.StartSubscription;

public sealed class StartSubscriptionValidator : AbstractValidator<StartSubscriptionCommand>
{
    public StartSubscriptionValidator() => RuleFor(x => x.CheckoutSessionId).NotEmpty();
}
