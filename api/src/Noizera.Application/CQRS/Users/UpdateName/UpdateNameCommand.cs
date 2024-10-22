using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.UpdateName;

public sealed record UpdateNameCommand(
    string NewName,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<UpdateNameCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateNameCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            user.UpdateName(request.NewName);

            await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
