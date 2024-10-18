using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Infrastructure.Emails;
using Noizera.Shared.Infrastructure.Subscriptions;

namespace Noizera.BackgroundJobs.Handlers;

public class SubscriptionRenewalPlannedEventHandler(SubscriptionStripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<SubscriptionRenewalPlannedEvent>>
{
    public async Task Handle(DomainEventNotification<SubscriptionRenewalPlannedEvent> notification, CancellationToken cancellationToken)
    {
        //Check if the stripe subscription is updated
        //If updated then create a new userSubscription and set IsActive = false for current one
    }
}
