using Microsoft.EntityFrameworkCore;
using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.ListeningHistories;
using Noizera.Common.Domain.MusicSets;
using Noizera.Common.Domain.MusicSetSongs;
using Noizera.Common.Domain.Outbox;
using Noizera.Common.Domain.ProfileRelations;
using Noizera.Common.Domain.Profiles;
using Noizera.Common.Domain.Royalties;
using Noizera.Common.Domain.SavedMusicSets;
using Noizera.Common.Domain.SecretTokens;
using Noizera.Common.Domain.SongPreferences;
using Noizera.Common.Domain.Songs;
using Noizera.Common.Domain.SongSimilarities;
using Noizera.Common.Domain.Streams;
using Noizera.Common.Domain.Subscriptions;
using Noizera.Common.Domain.Terms;
using Noizera.Common.Domain.Users;
using Noizera.Common.Domain.UserSubscriptions;
using Noizera.Common.Domain.VerificationCodes;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Persistence.SQL;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    private const string numberIdSequence = "NumberIdSequence";

    public required DbSet<User> Users { get; set; }
    public required DbSet<PublicProfile> Profiles { get; set; }
    public required DbSet<ProfileRelation> ProfileRelations { get; set; }
    public required DbSet<Subscription> Subscriptions { get; set; }
    public required DbSet<ListeningHistory> ListeningHistories { get; set; }
    public required DbSet<StreamInfo> Streams { get; set; }
    public required DbSet<UserSubscription> UserSubscriptions { get; set; }
    public required DbSet<Royalty> Royalties { get; set; }
    public required DbSet<Song> Songs { get; set; }
    public required DbSet<MusicSet> MusicSets { get; set; }
    public required DbSet<SavedMusicSet> SavedMusicSets { get; set; }
    public required DbSet<Album> Albums { get; set; }
    public required DbSet<Playlist> Playlists { get; set; }
    public required DbSet<AlbumCredit> AlbumCredits { get; set; }
    public required DbSet<MusicSetSong> MusicSetSongs { get; set; }
    public required DbSet<SongSimilarity> SongSimilarities { get; set; }
    public required DbSet<SongPreference> SongPreferences { get; set; }
    public required DbSet<VerificationCode> VerificationCodes { get; set; }
    public required DbSet<SecretToken> SecretTokens { get; set; }
    public required DbSet<OutboxMessage> OutboxMessages { get; set; }
    public required DbSet<TermsOfUse> Terms { get; set; }

    public virtual async Task InsertAsync<TEntity>(TEntity entity, CancellationToken ct)
        where TEntity : BaseEntity
    {
        _ = await AddAsync(entity, ct).ConfigureAwait(false);
        _ = await SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync<TEntity>(TEntity entity, CancellationToken ct)
        where TEntity : BaseEntity
    {
        //if (entity is Entity entityWithDates)
        //{
        //    //entityWithDates.SetLastModifiedOnAsNow();
        //}

        _ = Update(entity);
        _ = await SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<DomainEvent> domainEvents = ChangeTracker.Entries<Entity>()
           .SelectMany(entry => entry.Entity.PopDomainEvents())
           .ToList();

        await SaveDomainEventsAsync(domainEvents, cancellationToken).ConfigureAwait(false);

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<long> GetNextNumberIdSequenceValueAsync(CancellationToken ct)
    {
        FormattableString sql = $"""
            SELECT nextval('"public"."NumberIdSequence"')
        """;
        var query = Database.SqlQuery<long>(sql);
        var result = await query.ToListAsync(ct).ConfigureAwait(false);

        return result[0];
    }

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        _ = modelBuilder.HasSequence<long>(numberIdSequence).IncrementsBy(1).StartsAt(1000);

        _ = modelBuilder.HasPostgresExtension("pg_trgm");
    }

    private async Task SaveDomainEventsAsync(List<DomainEvent> domainEvents, CancellationToken ct = default)
    {
        var outboxMessages = domainEvents.Select(OutboxConverter.ConvertToOutboxMessage);
        await OutboxMessages.AddRangeAsync(outboxMessages, ct).ConfigureAwait(false);
    }
}

public static class AppDbExtensions
{
    public static async Task<TEntityExtended?> FirstOrDefaultByPublicIdAsync<TEntityExtended>(this DbSet<TEntityExtended> dbSet, string publicId, CancellationToken ct)
        where TEntityExtended : EntityExtended
        => await dbSet.FirstOrDefaultAsync(x => EF.Functions.ILike(x.PublicId, publicId), ct).ConfigureAwait(false);
}