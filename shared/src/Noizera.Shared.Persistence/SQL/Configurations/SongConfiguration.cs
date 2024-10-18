using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        ConfigurationHelper.ConfigureEntityExtended(builder);

        _ = builder.Property(e => e.Title)
            .HasMaxLength(500);

        _ = builder.Property(e => e.AudioFileMongoId);

        _ = builder.Property(e => e.OriginalFileName);

        _ = builder.Property(e => e.OriginalFileExtension);

        _ = builder.Property(e => e.Danceability);

        _ = builder.Property(e => e.Energy);

        _ = builder.Property(e => e.Key);

        _ = builder.Property(e => e.Scale);

        _ = builder.Property(e => e.BPM);

        _ = builder.Property(e => e.IsPublic);

        _ = builder.HasOne(e => e.Owner)
            .WithMany(e => e.Songs)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired();
    }
}
