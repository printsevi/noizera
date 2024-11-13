using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.AlbumCredits;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class AlbumCreditConfiguration : IEntityTypeConfiguration<AlbumCredit>
{
    public void Configure(EntityTypeBuilder<AlbumCredit> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .HasOne(e => e.Profile)
            .WithMany(e => e.AlbumCredits)
            .HasForeignKey(e => e.ProfileId)
            .IsRequired(false);

        _ = builder
            .HasOne(e => e.Album)
            .WithMany(e => e.AlbumCredits)
            .HasForeignKey(e => e.AlbumId)
            .IsRequired(true);
    }
}
