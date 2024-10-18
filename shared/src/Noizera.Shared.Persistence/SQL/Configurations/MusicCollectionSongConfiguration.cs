using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.MusicCollectionSongs;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class MusicCollectionSongConfiguration : IEntityTypeConfiguration<MusicCollectionSong>
{
    public void Configure(EntityTypeBuilder<MusicCollectionSong> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasOne(p => p.Song)
            .WithMany(t => t.MusicCollectionSongs)
            .HasForeignKey(p => p.SongId);

        _ = builder.HasOne(p => p.MusicCollection)
            .WithMany(t => t.MusicCollectionSongs)
            .HasForeignKey(p => p.MusicCollectionId);

        _ = builder.Property(e => e.SongId);

        _ = builder.Property(e => e.MusicCollectionId);

        _ = builder.Property(e => e.Sequence);

        _ = builder.HasIndex(x => new { x.MusicCollectionId, x.SongId }).IsUnique();
    }
}
