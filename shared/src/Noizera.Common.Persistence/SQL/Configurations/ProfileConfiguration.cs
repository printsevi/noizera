using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<PublicProfile>
{
    public void Configure(EntityTypeBuilder<PublicProfile> builder)
    {
        ConfigurationHelper.ConfigureEntityExtended(builder);

        _ = builder.HasIndex(e => e.Username).IsUnique();
        _ = builder.Property(e => e.Username).IsRequired(true);

        _ = builder.Property(e => e.Name);

        _ = builder.Property(e => e.ProfileType)
            .HasConversion<string>();

        _ = builder.Property(e => e.Bio);
    }
}
