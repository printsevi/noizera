using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<PublicProfile>
{
    public void Configure(EntityTypeBuilder<PublicProfile> builder)
    {
        ConfigurationHelper.ConfigureEntityExtended(builder);

        _ = builder.Property(e => e.Name);

        _ = builder.Property(e => e.ProfileType)
            .HasConversion<string>();

        _ = builder.Property(e => e.Description);
    }
}
