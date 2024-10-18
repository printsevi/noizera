using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.MusicSets;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class AlbumConfiguration : IEntityTypeConfiguration<Album>
{
    public void Configure(EntityTypeBuilder<Album> builder)
    {
        _ = builder.Property(e => e.AlbumStatus)
            .HasConversion<string>();
    }
}
