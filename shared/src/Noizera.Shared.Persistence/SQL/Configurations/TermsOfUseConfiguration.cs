using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Terms;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class TermsOfUseConfiguration : IEntityTypeConfiguration<TermsOfUse>
{
    public void Configure(EntityTypeBuilder<TermsOfUse> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.EffectiveDate);
    }
}
