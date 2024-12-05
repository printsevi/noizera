using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.MusicSetSongs;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

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

        _ = builder.HasIndex(x => new { x.MusicSetId, x.SongId }).IsUnique();
    }
}
