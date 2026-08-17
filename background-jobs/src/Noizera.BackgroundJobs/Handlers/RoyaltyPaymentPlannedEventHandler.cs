using MediatR;
using Microsoft.EntityFrameworkCore;
using Noizera.BackgroundJobs.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Domain.Royalties;
using Noizera.Common.Persistence.SQL;

namespace Noizera.BackgroundJobs.Handlers;

internal sealed class RoyaltyPaymentPlannedEventHandler(AppDbContext db)
    : INotificationHandler<DomainEventNotification<RoyaltyPaymentPlannedEvent>>
{
    public async Task Handle(DomainEventNotification<RoyaltyPaymentPlannedEvent> notification, CancellationToken cancellationToken)
    {
        var streams = await db.Streams
            .Include(x => x.Song)
            .Where(x => x.UserId == notification.DomainEvent.UserId
                && DateOnly.FromDateTime(x.StreamedAt.Date) <= DateOnly.FromDateTime(notification.DomainEvent.EndDate.Date)
                && DateOnly.FromDateTime(x.StreamedAt.Date) > DateOnly.FromDateTime(notification.DomainEvent.EffectiveDate.Date))
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

        if (fullTime <= 0)
        {
            return;
        }

        float amountToPay = (float)notification.DomainEvent.SubscriptionPrice * notification.DomainEvent.RoyaltyShare;
        float amountLeft = amountToPay;
        foreach (var pair in dict.OrderByDescending(x => x.Value))
        {
            if (amountLeft <= 0)
            {
                break;
            }

            float share = (float)pair.Value / fullTime;
            float amount = amountToPay * share;

            if (amountLeft <= amount)
            {
                amount = amountLeft;
            }

            amountLeft -= amount;

            Royalty royalty = Royalty.New(notification.DomainEvent.UserId, pair.Key, amount);
            _ = db.Royalties.Add(royalty);
        }
    }
}
