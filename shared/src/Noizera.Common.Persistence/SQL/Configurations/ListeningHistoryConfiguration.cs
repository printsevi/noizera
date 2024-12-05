using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.ListeningHistories;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class ListeningHistoryConfiguration : IEntityTypeConfiguration<ListeningHistory>
{
    public void Configure(EntityTypeBuilder<ListeningHistory> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasIndex(e => new { e.ListenerUserId, e.SongId })
            .IsUnique();

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
    }
}
