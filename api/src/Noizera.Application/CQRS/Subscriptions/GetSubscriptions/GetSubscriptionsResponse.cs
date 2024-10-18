namespace Noizera.Application.CQRS.Subscriptions.GetSubscriptions;

public sealed record GetSubscriptionsResponse(ICollection<SubscriptionResponse> Subscriptions);

public sealed record SubscriptionResponse(
    Guid SubscriptionId,
    string SubscriptionType,
    string Title,
    decimal PriceInEuro,
    IReadOnlyCollection<string> ProfileTypes,
    short? FreeTrialInDays,
    bool IsAnnual);
