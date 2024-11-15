using MediatR;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Subscriptions.GetUserSubscriptions;

public sealed record GetUserSubscriptionsQuery(Guid UserId)
    : IAuthorizeableRequest<GetUserSubscriptionsResponse>
{
    public sealed class Handler(
        IUserSubscriptionRepository userSubscriptionRepository)
        : IRequestHandler<GetUserSubscriptionsQuery, GetUserSubscriptionsResponse>
    {
        public async Task<GetUserSubscriptionsResponse> Handle([NotNull] GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await userSubscriptionRepository.GetAllAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            var result = subscriptions.Select(x => x.Subscription.SubscriptionType).Distinct();

            return new(result);
        }
    }
}
