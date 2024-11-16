using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.SongSimilarities;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SongSimilarityConfiguration : IEntityTypeConfiguration<SongSimilarity>
{
    public void Configure(EntityTypeBuilder<SongSimilarity> builder)
    {
        ConfigurationHelper.ConfigureBaseEntity(builder);

        _ = builder.HasKey(e => new { e.FirstSongId, e.SecondSongId });

        _ = builder.Property(e => e.Value);

        _ = builder.Property(e => e.LastProcessedOn);
    }
}
