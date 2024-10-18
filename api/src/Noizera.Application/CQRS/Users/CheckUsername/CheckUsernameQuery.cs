using MediatR;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.CheckUsername;

public sealed record CheckUsernameQuery(
    Guid UserId,
    string Username)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IProfileUniquenessChecker profileUniquenessChecker)
        : IRequestHandler<CheckUsernameQuery, Unit>
    {
        public async Task<Unit> Handle([NotNull] CheckUsernameQuery request, CancellationToken cancellationToken)
        {
            await PublicProfile.VerifyUsernameAsync(request.Username, profileUniquenessChecker, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
