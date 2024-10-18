using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.SongPreferences;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class SongPreferenceConfiguration : IEntityTypeConfiguration<SongPreference>
{
    public void Configure(EntityTypeBuilder<SongPreference> builder)
    {
        ConfigurationHelper.ConfigureBaseEntity(builder);

        _ = builder.HasKey(e => new { e.UserId, e.SongId });

        _ = builder.Property(e => e.Value);

        _ = builder.Property(e => e.LastProcessedOn);
    }
}
