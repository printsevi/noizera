using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.SecretTokens;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class SecretTokenConfiguration : IEntityTypeConfiguration<SecretToken>
{
    public void Configure(EntityTypeBuilder<SecretToken> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Token);

        _ = builder.Property(e => e.TokenType)
            .HasConversion<string>();

        _ = builder.Property(e => e.IsRevoked);

        _ = builder
            .HasOne(e => e.User)
            .WithMany(e => e.SecretTokens)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);

        _ = builder.Property(e => e.ExpireAt);
    }
}
