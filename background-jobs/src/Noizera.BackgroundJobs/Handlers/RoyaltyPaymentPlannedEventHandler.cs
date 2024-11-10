using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Domain.Royalties;
using Noizera.Shared.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

public class RoyaltyPaymentPlannedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<RoyaltyPaymentPlannedEvent>>
{
    public async Task Handle(DomainEventNotification<RoyaltyPaymentPlannedEvent> notification, CancellationToken cancellationToken)
    {
        var streams = await db.Streams
            .Include(x => x.Song)
            .Where(x => x.UserId == notification.DomainEvent.UserId
                && DateOnly.FromDateTime(x.CreatedAt.Date) <= DateOnly.FromDateTime(notification.DomainEvent.EndDate.Date)
                && DateOnly.FromDateTime(x.CreatedAt.Date) > DateOnly.FromDateTime(notification.DomainEvent.EffectiveDate.Date))
            .ToListAsync(cancellationToken);

        var dict = new Dictionary<Guid, int>();
        int fullTime = 0;
        foreach (var stream in streams)
        {
            var time = stream.TimeInSeconds;
            fullTime += time;
            if (dict.ContainsKey(stream.Song.OwnerId))
            {
                dict[stream.Song.OwnerId] += time;
            }
            else
            {
                dict.Add(stream.Song.OwnerId, time);
            }
        }

        var amountToPay = (float)notification.DomainEvent.SubscriptionPrice * notification.DomainEvent.RoyaltyShare;
        var amountLeft = amountToPay;
        foreach (var pair in dict)
        {
            if (amountLeft <= 0)
            {
                return;
            }

            var amount = amountToPay * (pair.Value / fullTime);

            if (amountLeft <= amount)
            {
                amount = amountLeft;
            }

            amountLeft -= amount;

            var royalty = Royalty.New(notification.DomainEvent.UserId, pair.Key, amount);
            await db.Royalties.AddAsync(royalty, cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
