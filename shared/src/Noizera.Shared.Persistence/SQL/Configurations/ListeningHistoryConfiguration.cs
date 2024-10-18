using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class ListeningHistoryConfiguration : IEntityTypeConfiguration<ListeningHistory>
{
    public void Configure(EntityTypeBuilder<ListeningHistory> builder)
    {
        ConfigurationHelper.ConfigureBaseEntity(builder);

        _ = builder.HasKey(e => new { e.ListenerUserId, e.SongId });

        _ = builder.Property(e => e.ListeningTimeInSeconds);

        _ = builder
            .HasOne(e => e.Listener)
            .WithMany(e => e.ListeningHistories)
            .HasForeignKey(e => e.ListenerUserId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.Song)
            .WithMany(e => e.ListeningHistories)
            .HasForeignKey(e => e.SongId)
            .IsRequired(true);

        _ = builder.Property(e => e.StreamCount);

        _ = builder.Property(e => e.FirstStreamed);

        _ = builder.Property(e => e.LastStreamed);

        _ = builder.Property(e => e.SkipCount);

        _ = builder.Property(e => e.FirstSkipped);

        _ = builder.Property(e => e.LastSkipped);
    }
}
