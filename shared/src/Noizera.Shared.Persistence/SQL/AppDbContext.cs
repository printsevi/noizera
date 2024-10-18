using Microsoft.EntityFrameworkCore;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Domain.MusicCollectionCredits;
using Noizera.Shared.Domain.MusicCollectionSongs;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Outbox;
using Noizera.Shared.Domain.ProfileRelations;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Royalties;
using Noizera.Shared.Domain.SavedMusicSets;
using Noizera.Shared.Domain.SecretTokens;
using Noizera.Shared.Domain.SongCredits;
using Noizera.Shared.Domain.SongPreferences;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Domain.SongSimilarities;
using Noizera.Shared.Domain.Streams;
using Noizera.Shared.Domain.Subscriptions;
using Noizera.Shared.Domain.Terms;
using Noizera.Shared.Domain.Users;
using Noizera.Shared.Domain.UserSubscriptions;
using Noizera.Shared.Domain.VerificationCodes;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Persistence.SQL;

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
    public DbSet<MusicSet> MusicCollections { get; set; }
    public DbSet<SavedMusicSet> SavedMusicCollections { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<MusicCollectionCredit> MusicCollectionCredits { get; set; }
    public DbSet<SongCredit> SongCredits { get; set; }
    public DbSet<MusicCollectionSong> MusicCollectionSongs { get; set; }
    public DbSet<SongSimilarity> SongSimilarities { get; set; }
    public DbSet<SongPreference> SongPreferences { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<SecretToken> RefreshTokens { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<TermsOfUse> Terms { get; set; }

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