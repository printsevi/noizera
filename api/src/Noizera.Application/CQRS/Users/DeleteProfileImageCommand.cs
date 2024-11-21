using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Persistence.S3;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users;

public sealed record DeleteProfileImageCommand(
    Guid UserId)
    : IAuthorizeableRequest<Unit>
{
    public sealed class Handler(
        AppDbContext db,
        S3Context s3Context)
        : IRequestHandler<DeleteProfileImageCommand, Unit>
    {
        public async Task<Unit> Handle([NotNull] DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await db.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken).ConfigureAwait(false)
                ?? throw new AppException($"User not found", ErrorType.NotFound);

            await s3Context.DeleteProfileImageAsync(user.Profile.PublicId, cancellationToken).ConfigureAwait(false);

            user.DeleteProfileImage();

            await db.UpdateAsync(user.Profile, cancellationToken).ConfigureAwait(false);

            return Unit.Value;
        }
    }
}
