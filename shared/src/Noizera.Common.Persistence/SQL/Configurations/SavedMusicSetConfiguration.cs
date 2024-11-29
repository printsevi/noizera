using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.SavedMusicSets;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class SavedMusicSetConfiguration : IEntityTypeConfiguration<SavedMusicSet>
{
    public void Configure(EntityTypeBuilder<SavedMusicSet> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .HasOne(e => e.User)
            .WithMany(e => e.SavedMusicSets)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.MusicSet)
            .WithMany(e => e.UserLibraries)
            .HasForeignKey(e => e.MusicSetId)
            .IsRequired(true);
    }
}
