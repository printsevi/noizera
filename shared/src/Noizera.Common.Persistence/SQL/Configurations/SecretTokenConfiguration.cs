using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.SecretTokens;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SecretTokenConfiguration : IEntityTypeConfiguration<SecretToken>
{
    public void Configure(EntityTypeBuilder<SecretToken> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Token);

        _ = builder.Property(e => e.TokenType)
            .HasMaxLength(Constants.EnumTypeMaxLength)
            .HasConversion<string>();

        _ = builder.Property(e => e.IsRevoked);

        _ = builder
            .HasOne(e => e.User)
            .WithMany(e => e.SecretTokens)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);
    }
}
