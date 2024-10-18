using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.VerificationCodes;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
{
    public void Configure(EntityTypeBuilder<VerificationCode> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Key);

        _ = builder.Property(e => e.Code);

        _ = builder.Property(e => e.Invalid);

        _ = builder.Property(e => e.Verified);

        _ = builder.Property(e => e.CreatedAt);

        _ = builder.Property(e => e.ExpireAt);
    }
}
