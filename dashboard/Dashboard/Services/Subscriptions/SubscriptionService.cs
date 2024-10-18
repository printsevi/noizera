using Dashboard.Models;
using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Persistence.SQL;

namespace Dashboard.Services.Subscriptions;

public class SubscriptionService(AppDbContext db)
{
    public async Task<PagedResult<Subscription>> GetSubscriptionsAsync(string? name, int page)
    {
        int pageSize = 5;

        if (name != null)
        {
            return await db.Subscriptions
                .Where(x => EF.Functions.Like(x.SubscriptionType, name))
                .OrderBy(p => p.Id)
                .GetPagedAsync(page, pageSize);
        }

        return await db.Subscriptions
                .OrderBy(p => p.Id)
                .GetPagedAsync(page, pageSize);
    }

    public async Task<Subscription> GetAsync(Guid id)
    {
        return await db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new KeyNotFoundException();
    }

    public async Task DeleteAsync(Subscription subscription)
    {
        var result = await db.Subscriptions.FirstOrDefaultAsync(p => p.Id == subscription.Id)
            ?? throw new KeyNotFoundException("Subscription not found");

        db.Subscriptions.Remove(result);
        await db.SaveChangesAsync();
    }

    public async Task CreateAsync(Subscription subscription)
    {
        var result = await db.Subscriptions.AddAsync(subscription);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        db.Update(subscription);
        await db.SaveChangesAsync();
    }
}
