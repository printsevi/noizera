namespace Noizera.Application.CQRS.Subscriptions.GetUserSubscriptions;

public sealed record GetUserSubscriptionsResponse(IEnumerable<string> ActiveSubscriptions);
