using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Noizera.Common.Domain.ProfileRelations;
using Noizera.Common.Persistence.SQL.Configurations.Common;

namespace Noizera.Common.Persistence.SQL.Configurations;

internal sealed class ProfileRelationConfiguration : IEntityTypeConfiguration<ProfileRelation>
{
    public void Configure(EntityTypeBuilder<ProfileRelation> builder)
    {
        ConfigurationHelper.ConfigureEntity(builder);

        _ = builder.HasIndex(pr => new { pr.FollowerProfileId, pr.FollowingProfileId })
            .IsUnique();

        _ = builder
            .HasOne(e => e.FollowerProfile)
            .WithMany(e => e.Followings)
            .HasForeignKey(e => e.FollowerProfileId)
            .IsRequired(true);

        _ = builder
            .HasOne(e => e.FollowingProfile)
            .WithMany(e => e.Followers)
            .HasForeignKey(e => e.FollowingProfileId)
            .IsRequired(true);
    }
}
