using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class UserCreatedEventHandler(AppDbContext db, BrevoService emailService)
    : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    public async Task Handle(DomainEventNotification<UserCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(a => a.Profile)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.UserId, cancellationToken: cancellationToken).ConfigureAwait(false)
            ?? throw new ArgumentException($"User {notification.DomainEvent.UserId} is not found");

        await emailService.SendTransactionalEmailAsync(
                user.Email,
                user.Profile.Name,
                BrevoIds.UserCreated,
                cancellationToken
        ).ConfigureAwait(false);
    }
}
