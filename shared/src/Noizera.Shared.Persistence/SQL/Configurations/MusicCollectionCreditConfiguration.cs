using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.MusicCollectionCredits;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class MusicCollectionCreditConfiguration : IEntityTypeConfiguration<MusicCollectionCredit>
{
    public void Configure(EntityTypeBuilder<MusicCollectionCredit> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .HasOne(e => e.Profile)
            .WithMany(e => e.MusicCollectionCredits)
            .HasForeignKey(e => e.ProfileId)
            .IsRequired(false);

        _ = builder
            .HasOne(e => e.MusicCollection)
            .WithMany(e => e.Credits)
            .HasForeignKey(e => e.MusicCollectionId)
            .IsRequired(true);

        _ = builder.Property(e => e.ProfileName).IsRequired(false);

        _ = builder.Property(e => e.ProfileType).IsRequired(false);

        _ = builder.HasIndex(x => new { x.MusicCollectionId, x.ProfileId }).IsUnique();
    }
}
