using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Title);

        _ = builder.Property(e => e.Price);

        _ = builder.Property(e => e.StripePriceId);

        _ = builder.Property(e => e.IsDisabled);

        _ = builder.Property(e => e.SubscriptionType);

        _ = builder.Ignore(e => e.IsDeleting);

        _ = builder.HasMany(e => e.UserSubscriptions)
            .WithOne(e => e.Subscription)
            .HasForeignKey(e => e.SubscriptionId)
            .IsRequired();
    }
}
