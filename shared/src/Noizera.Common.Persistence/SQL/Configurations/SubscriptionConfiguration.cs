using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Title).HasMaxLength(100);

        _ = builder.Property(e => e.SubscriptionType)
            .HasMaxLength(Constants.EnumTypeMaxLength);

        _ = builder.Ignore(e => e.IsDeleting);

        _ = builder.HasMany(e => e.UserSubscriptions)
            .WithOne(e => e.Subscription)
            .HasForeignKey(e => e.SubscriptionId)
            .IsRequired();
    }
}
