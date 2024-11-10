using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Persistence.SQL.Configurations.Common;

internal static class ConfigurationHelper
{
    public static void ConfigureBaseEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
    }

    public static void ConfigureEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        ConfigureBaseEntity<TEntity>(builder);

        _ = builder.Property(e => e.Id)
            .ValueGeneratedNever();

        _ = builder.HasKey(e => e.Id);

        _ = builder.Property(e => e.CreatedAt);
    }

    public static void ConfigureEntityExtended<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : EntityExtended
    {
        ConfigureEntity<TEntity>(builder);

        _ = builder.HasIndex(e => e.PublicId).IsUnique();
        _ = builder.Property(e => e.PublicId).IsRequired(false);
    }
}
