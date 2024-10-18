using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Infrastructure.Emails;
using Noizera.Shared.Infrastructure.Subscriptions;

namespace Noizera.BackgroundJobs.Handlers;

public class CheckoutSessionCreatedEventHandler(SubscriptionStripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<CheckoutSessionCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<CheckoutSessionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var sessionStatus = await subscriptionService.GetCheckoutSessionStatus(notification.DomainEvent.StripeSessionId);
        if (sessionStatus is null)
        {

        }
        else if (sessionStatus == "open")
        {
            await Task.Delay(5000);//5 sec
        }
        else if (sessionStatus == "expired")
        {

        }
    }
}
