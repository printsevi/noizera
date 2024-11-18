using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.Common.Contracts.Errors;
using Noizera.Common.Contracts.Security;
using Noizera.Common.Domain.Common;
using Noizera.Common.Persistence.SQL;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Application.CQRS.Users.GetMyUser;

public sealed record GetMyUserQuery(Guid UserId)
    : IAuthorizeableRequest<GetMyUserResponse>
{
    public sealed class Handler(
        AppDbContext db)
        : IRequestHandler<GetMyUserQuery, GetMyUserResponse>
    {
        public async Task<GetMyUserResponse> Handle([NotNull] GetMyUserQuery request, CancellationToken cancellationToken)
        {
            var currentTime = SystemClock.UtcNow;

            var result = await db.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new GetMyUserResponse(
                    u.Profile.ProfileType.ToString(),
                    u.Profile.PublicId,
                    u.Profile.Name,
                    u.ActiveSubscriptionTypes,
                    u.Songs.Count,
                    db.Streams
                        .Where(s => s.UserId == request.UserId && s.StreamedAt >= currentTime.AddDays(-1))
                        .Sum(s => s.TimeInSeconds) >= 20 * 60,
                    db.Streams
                        .Where(s => s.UserId == request.UserId && s.StreamedAt >= currentTime.AddDays(-7))
                        .Sum(s => s.TimeInSeconds) >= 60 * 60,
                    db.Streams
                        .Where(s => s.UserId == request.UserId && s.StreamedAt >= currentTime.AddMonths(-1))
                        .Sum(s => s.TimeInSeconds) >= 2 * 60 * 60,
                    db.Streams
                        .Where(s => s.UserId == request.UserId && s.StreamedAt >= currentTime.AddMonths(-6))
                        .Sum(s => s.TimeInSeconds) >= 4 * 60 * 60))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false)
                ?? throw new AppException($"User {request.UserId} not found", ErrorType.NotFound);

            return result;
        }
    }
}
