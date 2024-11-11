using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Contracts.Errors;
using Noizera.Shared.Contracts.Security;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.UpdateProfileType;

public sealed record UpdateProfileTypeCommand(
    string NewProfileType,
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<UpdateProfileTypeCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] UpdateProfileTypeCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse(request.NewProfileType, out ProfileType newProfileType))
            {
                throw new AppException($"{request.NewProfileType} is unknown", ErrorType.BadRequest);
            }

            var user = await db.Users
                .Include(u => u.Profile)
                .Include(u => u.Songs)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            user.UpdateProfileType(newProfileType);

            await db.UpdateAsync(user, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
