using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Streams;
using Noizera.Shared.Persistence.SQL.Configurations.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations;

internal sealed class StreamInfoConfiguration : IEntityTypeConfiguration<StreamInfo>
{
    public void Configure(EntityTypeBuilder<StreamInfo> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.Property(e => e.TimeInSeconds).IsRequired(true);

        _ = builder
            .HasOne(e => e.User)
            .WithMany(e => e.Streams)
            .HasForeignKey(e => e.UserId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.Song)
            .WithMany(e => e.Streams)
            .HasForeignKey(e => e.SongId)
            .IsRequired(true);
    }
}
