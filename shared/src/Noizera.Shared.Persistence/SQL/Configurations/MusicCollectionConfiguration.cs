using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class MusicCollectionConfiguration : IEntityTypeConfiguration<MusicSet>
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
            .WithMany(e => e.MusicCollections)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired(true);
    }
}
