using MediatR;
using Noizera.Application.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Infrastructure.Subscriptions;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record GetSubscriptionPortalQuery(Guid UserId)
    : IAuthorizeableRequest<UrlResponse>
{
    public sealed class Handler(
        IUserRepository userRepository,
        SubscriptionStripeService subscriptionService)
        : IRequestHandler<GetSubscriptionPortalQuery, UrlResponse>
    {
        public async Task<UrlResponse> Handle([NotNull] GetSubscriptionPortalQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User is not found by UserId: {request.UserId}", ErrorType.NotFound);

            if (string.IsNullOrWhiteSpace(user.CustomerStripeId))
            {
                throw new AppException($"Customer stripe Id is not found by UserId: {request.UserId}", ErrorType.BadRequest);
            }

            var result = await subscriptionService.GetSubscriptionPortalUrlAsync(user, cancellationToken).ConfigureAwait(false);
            return result is null
                ? throw new AppException($"Error while creating a billing portal for UserId: {request.UserId}", ErrorType.Internal)
                : new(result);
        }
    }
}
