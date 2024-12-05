using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.Common;

namespace Noizera.Common.Persistence.SQL.Configurations.Common;

internal static class ConfigurationHelper
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
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
    }

    public static void ConfigureEntityExtended<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : EntityExtended
    {
        ConfigureEntity<TEntity>(builder);

        _ = builder.HasIndex(e => e.PublicId)
            .IsUnique();
        _ = builder.Property(e => e.PublicId)
            .HasMaxLength(Constants.PublicIdMaxLength)
            .IsRequired(true);
    }
}
