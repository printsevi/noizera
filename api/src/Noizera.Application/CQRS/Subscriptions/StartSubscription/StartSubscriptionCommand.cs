using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Infrastructure.Subscriptions;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Subscriptions.StartSubscription;

public sealed record StartSubscriptionCommand(
    string CheckoutSessionId,
    Guid UserId)
    : IAuthorizeableRequest<StartSubscriptionResponse>
{
    public sealed class Handler(
        IUserSubscriptionRepository userSubscriptionRepository,
        SubscriptionStripeService subscriptionService)
        : IRequestHandler<StartSubscriptionCommand, StartSubscriptionResponse>
    {
        public async Task<StartSubscriptionResponse> Handle([NotNull] StartSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var userSubscription = await userSubscriptionRepository.GetByCheckoutSessionIdAsync(request.CheckoutSessionId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User subscription is not found by {request.CheckoutSessionId}", ErrorType.NotFound);

            if (!userSubscription.CheckoutSessionIsProcessed)
            {
                await subscriptionService.TryActivateSubscriptionAsync(request.CheckoutSessionId, userSubscription, cancellationToken).ConfigureAwait(false);
                await userSubscriptionRepository.UpdateAsync(userSubscription, cancellationToken).ConfigureAwait(false);
            }

            return new(userSubscription.Subscription.SubscriptionType.ToString());
        }
    }
}
