using MediatR;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Subscriptions;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class CheckoutSessionCreatedEventHandler(SubscriptionStripeService subscriptionService)
    : INotificationHandler<DomainEventNotification<CheckoutSessionCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<CheckoutSessionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        string? sessionStatus = await subscriptionService.GetCheckoutSessionStatus(notification.DomainEvent.StripeSessionId, cancellationToken).ConfigureAwait(false);
        if (sessionStatus is null)
        {

        }
        //else if (sessionStatus == "open")
        //{
        //    await Task.Delay(5000);//5 sec
        //}
        //else if (sessionStatus == "expired")
        //{

        //}
    }
}
