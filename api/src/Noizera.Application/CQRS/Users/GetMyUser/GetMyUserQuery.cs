using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.GetMyUser;

public sealed record GetMyUserQuery(Guid UserId)
    : IAuthorizeableRequest<GetMyUserResponse>
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<GetMyUserQuery, GetMyUserResponse>
    {
        public async Task<GetMyUserResponse> Handle([NotNull] GetMyUserQuery request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetWithSubscriptionsAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User {request.UserId} not found", ErrorType.NotFound);

            return new(
                result.Profile!.ProfileType.ToString(), 
                result.Profile!.PublicId, 
                result.Profile!.Name,
                result.ActiveSubscriptionTypes);
        }
    }
}
