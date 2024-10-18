using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Outbox;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class OutboxConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.Type);

        _ = builder.Property(e => e.Data);

        _ = builder.Property(e => e.OccurredOn);

        _ = builder.Property(e => e.ProcessedOn);

        _ = builder.Property(e => e.ProcessAfter);

        _ = builder.Property(e => e.FailedOn);

        _ = builder.Property(e => e.ErrorText);

        _ = builder.Property(e => e.IsRealTime);

        _ = builder.Property(e => e.Retries);
    }
}
