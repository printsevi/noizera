using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Infrastructure.Emails;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class AlbumReleasedEventHandler(
    BrevoService emailService,
    AppDbContext db)
    : INotificationHandler<DomainEventNotification<AlbumReleasedEvent>>
{
    public async Task Handle(DomainEventNotification<AlbumReleasedEvent> notification, CancellationToken cancellationToken)
    {
        var album = await db.Albums
            .Include(a => a.Owner)
                .ThenInclude(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == notification.DomainEvent.AlbumId, cancellationToken: cancellationToken).ConfigureAwait(false)
            ?? throw new ArgumentException($"Album {notification.DomainEvent.AlbumId} is not found");

        await emailService.SendTransactionalEmailAsync(
            album.Owner.Email,
            album.Owner.Profile.Name,
            BrevoIds.AlbumReleased,
            cancellationToken,
            new Dictionary<string, object>() {
                { "albumTitle", album.Title },
                { "albumLink", $"{Constants.Domain}collections/{album.PublicId}" }
            }
        ).ConfigureAwait(false);
    }
}
