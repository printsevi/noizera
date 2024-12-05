using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Domain.Users;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasIndex(e => e.Email).IsUnique();
        _ = builder.Property(e => e.Email).IsRequired(true);

        _ = builder.Property(e => e.Roles).HasMaxLength(150);

        _ = builder.HasOne(e => e.Profile)
            .WithOne(e => e.User)
            .HasForeignKey<PublicProfile>(e => e.UserId)
            .IsRequired(false);

        _ = builder.HasMany(e => e.Subscriptions)
            .WithOne(e => e.User);

        _ = builder.HasMany(e => e.Subscriptions)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        _ = builder.HasMany(e => e.Subscriptions)
            .WithOne(e => e.User);
    }
}
