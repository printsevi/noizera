using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Royalties;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class RoyaltyConfiguration : IEntityTypeConfiguration<Royalty>
{
    public void Configure(EntityTypeBuilder<Royalty> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder
            .HasOne(e => e.Payer)
            .WithMany(e => e.RoyaltiesReceived)
            .HasForeignKey(e => e.PayerId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.Payee)
            .WithMany(e => e.RoyaltiesSent)
            .HasForeignKey(e => e.PayeeId)
            .IsRequired(true);
    }
}
