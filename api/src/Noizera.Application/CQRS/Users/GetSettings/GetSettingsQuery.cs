using MediatR;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Repositories;
using Noizera.Common.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.GetSettings;

public sealed record GetSettingsQuery(Guid UserId)
    : IAuthorizeableRequest<GetSettingsResponse>
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<GetSettingsQuery, GetSettingsResponse>
    {
        public async Task<GetSettingsResponse> Handle([NotNull] GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User {request.UserId} not found", ErrorType.NotFound);

            return new(
                user.Profile.Name,
                user.Profile.Bio,
                user.Profile.ImageOriginalName);
        }
    }
}
