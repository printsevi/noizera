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

    public DbSet<User> Users { get; set; }
    public DbSet<PublicProfile> Profiles { get; set; }
    public DbSet<ProfileRelation> ProfileRelations { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<ListeningHistory> ListeningHistories { get; set; }
    public DbSet<StreamInfo> Streams { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<Royalty> Royalties { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<MusicSet> MusicSets { get; set; }
    public DbSet<SavedMusicSet> SavedMusicSets { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<AlbumCredit> AlbumCredits { get; set; }
    public DbSet<MusicSetSong> MusicSetSongs { get; set; }
    public DbSet<SongSimilarity> SongSimilarities { get; set; }
    public DbSet<SongPreference> SongPreferences { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<SecretToken> SecretTokens { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<TermsOfUse> Terms { get; set; }

    public virtual async Task InsertAsync<TEntity>(TEntity entity, CancellationToken ct) 
        where TEntity : BaseEntity
    {
        _ = await AddAsync(entity, ct).ConfigureAwait(false);
        _ = await SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync<TEntity>(TEntity entity, CancellationToken ct)
        where TEntity : BaseEntity
    {
        if (entity is Entity entityWithDates)
        {
            //entityWithDates.SetLastModifiedOnAsNow();
        }

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