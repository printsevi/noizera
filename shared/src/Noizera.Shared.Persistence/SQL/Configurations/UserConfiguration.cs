using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasIndex(e => e.Email).IsUnique();
        _ = builder.Property(e => e.Email).IsRequired(true);

        _ = builder.Property(e => e.CustomerStripeId);

        _ = builder.Property(e => e.PasswordHash);

        _ = builder.Property(e => e.PasswordSalt);

        _ = builder.Property(e => e.Roles);

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
