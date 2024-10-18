using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.SavedMusicSets;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class SavedMusicSetConfiguration : IEntityTypeConfiguration<SavedMusicSet>
{
    public void Configure(EntityTypeBuilder<SavedMusicSet> builder)
    {
        ConfigurationHelper.ConfigureBaseEntity(builder);

        _ = builder.HasKey(x => new { x.UserId, x.MusicSetId });

        _ = builder
            .HasOne(e => e.User)
            .WithMany(e => e.SavedMusicCollections)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.MusicSet)
            .WithMany(e => e.UserLibraries)
            .HasForeignKey(e => e.MusicSetId)
            .IsRequired(true);
    }
}
