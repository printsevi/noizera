using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.SongCredits;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class SongCreditConfiguration : IEntityTypeConfiguration<SongCredit>
{
    public void Configure(EntityTypeBuilder<SongCredit> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .HasOne(e => e.Profile)
            .WithMany(e => e.SongCredits)
            .HasForeignKey(e => e.ProfileId)
            .IsRequired(false);

        _ = builder
            .HasOne(e => e.Song)
            .WithMany(e => e.Credits)
            .HasForeignKey(e => e.SongId)
            .IsRequired(true);
    }
}
