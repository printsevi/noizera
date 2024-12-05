using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSets;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        _ = builder.Property(e => e.AlbumStatus)
            .HasMaxLength(Constants.EnumTypeMaxLength)
            .HasConversion<string>();

        _ = builder.Property(e => e.AlbumReleaseDate)
            .HasColumnType("DATE");
    }
}
