using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.ExternalLinks;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class ExternalLinkConfiguration : IEntityTypeConfiguration<ExternalLink>
{
    public void Configure(EntityTypeBuilder<ExternalLink > builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .Property(e => e.Url)
            .HasMaxLength(100);

        _ = builder
            .HasOne(e => e.Profile)
            .WithMany(e => e.ExternalLinks)
            .HasForeignKey(e => e.ProfileId)
            .IsRequired(true);
    }
}
