using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.QueryResults;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.GetMyUser;

public sealed record GetMyUserQuery(Guid UserId)
    : IAuthorizeableRequest<MyUserQueryResult>
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<GetMyUserQuery, MyUserQueryResult>
    {
        public async Task<MyUserQueryResult> Handle([NotNull] GetMyUserQuery request, CancellationToken cancellationToken)
        {
            var result = await userRepository.GetMyUserAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User {request.UserId} not found", ErrorType.NotFound);

            return result;
        }
    }
}
