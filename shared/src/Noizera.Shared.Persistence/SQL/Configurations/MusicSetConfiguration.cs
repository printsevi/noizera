using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class MusicSetConfiguration : IEntityTypeConfiguration<MusicSet>
{
    public void Configure(EntityTypeBuilder<MusicSet> builder)
    {
        ConfigurationHelper.ConfigureEntityExtended(builder);

        _ = builder
            .HasDiscriminator<string>("CollectionType")
            .HasValue<MusicSet>("collection_base")
            .HasValue<Album>("collection_album")
            .HasValue<Playlist>("collection_playlist");

        _ = builder
            .HasOne(e => e.Owner)
            .WithMany(e => e.MusicSets)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired(true);
    }
}
