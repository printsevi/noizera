using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Terms;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class TermsOfUseConfiguration : IEntityTypeConfiguration<TermsOfUse>
{
    public void Configure(EntityTypeBuilder<TermsOfUse> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.EffectiveDate);
    }
}
