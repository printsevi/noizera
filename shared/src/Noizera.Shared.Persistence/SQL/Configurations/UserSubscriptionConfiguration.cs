using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.UserSubscriptions;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.CurrentPeriodStart);

        _ = builder.Property(e => e.CurrentPeriodEnd);

        _ = builder.Property(e => e.IsActive);
    }
}
