using MediatR;
using Microsoft.Extensions.Configuration;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Infrastructure.Subscriptions;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Subscriptions.Checkout;

public sealed record CheckoutCommand(
    Guid SubscriptionId,
    Guid UserId)
    : IAuthorizeableRequest<CheckoutResponse>
{
    public sealed class Handler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        SubscriptionStripeService subscriptionService)
        : IRequestHandler<CheckoutCommand, CheckoutResponse>
    {
        public async Task<CheckoutResponse> Handle([NotNull] CheckoutCommand request, CancellationToken cancellationToken)
        {
            var subscription = await subscriptionRepository.GetAsync(request.SubscriptionId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Subscription {request.SubscriptionId} is not found", ErrorType.NotFound);

            var user = await userRepository.GetWithSubscriptionsAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User {request.UserId} is not found", ErrorType.NotFound);

            if (user.HasActiveSubscriptionOfType(subscription.SubscriptionType))
            {
                throw new AppException($"A subscription of {subscription.SubscriptionType} is still active.", ErrorType.BadRequest);
            }

            if (string.IsNullOrWhiteSpace(user.CustomerStripeId))
            {
                await subscriptionService.CreateStripeCustomerAsync(user, cancellationToken).ConfigureAwait(false);
                await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            }

            var result = await subscriptionService.CreateCheckoutSessionAsync(user, subscription, cancellationToken).ConfigureAwait(false);

            return new(result);
        }
    }
}
