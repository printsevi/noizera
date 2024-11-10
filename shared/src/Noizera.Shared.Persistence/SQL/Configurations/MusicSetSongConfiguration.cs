using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.MusicSetSongs;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class MusicSetSongConfiguration : IEntityTypeConfiguration<MusicSetSong>
{
    public void Configure(EntityTypeBuilder<MusicSetSong> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasOne(p => p.Song)
            .WithMany(t => t.MusicSetSongs)
            .HasForeignKey(p => p.SongId);

        _ = builder.HasOne(p => p.MusicSet)
            .WithMany(t => t.MusicSetSongs)
            .HasForeignKey(p => p.MusicSetId);

        _ = builder.Property(e => e.SongId);

        _ = builder.Property(e => e.MusicSetId);

        _ = builder.Property(e => e.Sequence);

        _ = builder.HasIndex(x => new { x.MusicSetId, x.SongId }).IsUnique();
    }
}
