using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        ConfigurationHelper.ConfigureEntityExtended(builder);

        _ = builder.Property(e => e.Title)
            .HasMaxLength(100);

        _ = builder.Property(e => e.OriginalFileName)
            .HasMaxLength(Constants.OriginalFileMaxLength);

        _ = builder.Property(e => e.OriginalFileExtension)
            .HasMaxLength(10);

        _ = builder.Property(e => e.Scale).HasMaxLength(30);

        _ = builder.HasOne(e => e.Owner)
            .WithMany(e => e.Songs)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired();
    }
}
