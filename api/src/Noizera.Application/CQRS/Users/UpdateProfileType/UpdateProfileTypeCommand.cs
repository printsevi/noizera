using MediatR;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Repositories;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Profiles;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.UpdateProfileType;

public sealed record UpdateProfileTypeCommand(
    string NewProfileType,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        IUserRepository userRepository)
        : IRequestHandler<UpdateProfileTypeCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateProfileTypeCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAsync(request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            if (!Enum.TryParse(request.NewProfileType, out ProfileType newProfileType))
            {
                throw new AppException($"{request.NewProfileType} is unknown", ErrorType.BadRequest);
            }

            user.Profile!.SetProfileType(newProfileType);

            await userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
