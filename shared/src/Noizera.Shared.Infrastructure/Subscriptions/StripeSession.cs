namespace Noizera.Shared.Infrastructure.Subscriptions;

public record StripeSession(
    bool IsPaid,
    bool? IsTrial,
    string? SubscriptionId,
    DateTimeOffset? CurrentPeriodStart,
    DateTimeOffset? CurrentPeriodEnd);
