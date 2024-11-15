using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Domain.Royalties;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal class RoyaltyPaymentPlannedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<RoyaltyPaymentPlannedEvent>>
{
    public async Task Handle(DomainEventNotification<RoyaltyPaymentPlannedEvent> notification, CancellationToken cancellationToken)
    {
        var streams = await db.Streams
            .Include(x => x.Song)
            .Where(x => x.UserId == notification.DomainEvent.UserId
                && DateOnly.FromDateTime(x.CreatedAt.Date) <= DateOnly.FromDateTime(notification.DomainEvent.EndDate.Date)
                && DateOnly.FromDateTime(x.CreatedAt.Date) > DateOnly.FromDateTime(notification.DomainEvent.EffectiveDate.Date))
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        Dictionary<Guid, int> dict = [];
        int fullTime = 0;
        foreach (var stream in streams)
        {
            int time = stream.TimeInSeconds;
            fullTime += time;
            if (!dict.TryAdd(stream.Song.OwnerId, time))
            {
                dict[stream.Song.OwnerId] += time;
            }
        }

        float amountToPay = (float)notification.DomainEvent.SubscriptionPrice * notification.DomainEvent.RoyaltyShare;
        float amountLeft = amountToPay;
        foreach (var pair in dict)
        {
            if (amountLeft <= 0)
            {
                return;
            }

            float amount = amountToPay * (pair.Value / fullTime);

            if (amountLeft <= amount)
            {
                amount = amountLeft;
            }

            amountLeft -= amount;

            Royalty royalty = Royalty.New(notification.DomainEvent.UserId, pair.Key, amount);
            _ = await db.Royalties.AddAsync(royalty, cancellationToken).ConfigureAwait(false);
            _ = await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
