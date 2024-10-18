using MediatR;
using Noizera.Shared.Contracts.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Subscriptions.GetSubscriptions;

public sealed record GetSubscriptionsQuery(string? ProfileType)
    : IRequest<GetSubscriptionsResponse>
{
    public sealed class Handler(
        ISubscriptionRepository subscriptionRepository)
        : IRequestHandler<GetSubscriptionsQuery, GetSubscriptionsResponse>
    {
        public async Task<GetSubscriptionsResponse> Handle([NotNull] GetSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await subscriptionRepository.GetAllAsync(request.ProfileType, cancellationToken).ConfigureAwait(false);
            List<SubscriptionResponse> result = [];
            foreach (var subscription in subscriptions)
            {
                result.Add(new(
                    subscription.Id,
                    subscription.SubscriptionType.ToString(),
                    subscription.Title,
                    subscription.Price,
                    subscription.SplitProfileTypes,
                    subscription.FreeTrialInDays,
                    subscription.IsAnnual));
            }

            return new(result);
        }
    }
}
