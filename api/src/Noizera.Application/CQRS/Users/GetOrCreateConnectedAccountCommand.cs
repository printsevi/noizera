using MediatR;
using Microsoft.Extensions.Configuration;
using Noizera.Application.Common;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Infrastructure.Subscriptions;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record GetOrCreateConnectedAccountCommand(
    Guid UserId)
    : IAuthorizeableRequest<UrlResponse>
{
    public sealed class Handler(
        StripeService stripeService,
        AppDbContext db,
        IUserRepository userRepository,
        IConfiguration configuration)
        : IRequestHandler<GetOrCreateConnectedAccountCommand, UrlResponse>
    {
        public async Task<UrlResponse> Handle([NotNull] GetOrCreateConnectedAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User is not found by UserId: {request.UserId}", ErrorType.NotFound);

            if (user.ConnectedAccountStripeId is null)
            {
                var createdAccount = await stripeService.CreateConnectedAccountAsync(cancellationToken).ConfigureAwait(false)
                    ?? throw new AppException($"Stripe connected account is failed for user: {request.UserId}", ErrorType.Internal);
                user.ConnectStripeAccount(createdAccount.Id);
                await db.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
            }

            var account = await stripeService.GetConnectedAccountAsync(user.ConnectedAccountStripeId!, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Stripe connected account is not found by stripe id: {user.ConnectedAccountStripeId}", ErrorType.NotFound);

            if (account.Requirements.CurrentlyDue.Count == 0)
            {
                var loginLink = await stripeService.GetLoginAccountLinkAsync(user.ConnectedAccountStripeId!, cancellationToken).ConfigureAwait(false)
                    ?? throw new AppException($"Stripe connected account is not found by stripe id: {user.ConnectedAccountStripeId}", ErrorType.NotFound);

                return new(loginLink);
            }

            string host = configuration.GetSection("FrontendUrls")?.Get<string[]>()?.FirstOrDefault() ?? string.Empty;

            var onboardingLink = await stripeService.GetOnboardingConnectedAccountLinkAsync(
                user.ConnectedAccountStripeId!,
                new Uri($"{host}/system/onboarding-refresh"),
                new Uri(host),
                cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"Stripe onboarding link is not created by id: {user.ConnectedAccountStripeId}", ErrorType.Internal);

            return new(onboardingLink);
        }
    }
}
