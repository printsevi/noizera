using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Subscriptions;

namespace Noizera.BackgroundJobs.Handlers;

internal class SubscriptionRenewalPlannedEventHandler(SubscriptionStripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<SubscriptionRenewalPlannedEvent>>
{
#pragma warning disable IDE0060 // Remove unused parameter
    public void Handle(DomainEventNotification<SubscriptionRenewalPlannedEvent> notification, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var a = subscriptionService;
        throw new NotImplementedException(a.ToString());
        //Check if the stripe subscription is updated
        //If updated then create a new userSubscription and set IsActive = false for current one
    }

    Task INotificationHandler<DomainEventNotification<SubscriptionRenewalPlannedEvent>>.Handle(DomainEventNotification<SubscriptionRenewalPlannedEvent> notification, CancellationToken cancellationToken) => throw new NotImplementedException();
}
