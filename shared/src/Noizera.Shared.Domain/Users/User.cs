using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.Events;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Profiles;
using Noizera.Shared.Domain.Royalties;
using Noizera.Shared.Domain.SavedMusicSets;
using Noizera.Shared.Domain.SecretTokens;
using Noizera.Shared.Domain.Songs;
using Noizera.Shared.Domain.Streams;
using Noizera.Shared.Domain.UserSubscriptions;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.Users;

public sealed class User : Entity
{
    public string Email { get; private set; } = null!;
    public string? CustomerStripeId { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public byte[] PasswordSalt { get; private set; } = [];
    public string Roles { get; private set; } = null!;
    public short SongLimitToUpload { get; private set; }

    public PublicProfile Profile { get; private set; } = null!;
    public ICollection<Song> Songs { get; } = [];
    public ICollection<MusicSet> MusicSets { get; } = [];
    public ICollection<SavedMusicSet> SavedMusicSets { get; } = [];
    public ICollection<UserSubscription> Subscriptions { get; } = [];
    public ICollection<ListeningHistory> ListeningHistories { get; } = [];
    public ICollection<StreamInfo> Streams { get; } = [];
    public ICollection<Royalty> RoyaltiesReceived { get; } = [];
    public ICollection<Royalty> RoyaltiesSent { get; } = [];
    public ICollection<SecretToken> SecretTokens { get; } = [];

    private User(
        string email,
        string passwordHash,
        byte[] passwordSalt,
        string roles)
            : base()
    {
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Roles = roles;
    }

    public static async Task<User> CreateAsync(
        string email,
        string password,
        string roles,
        string profileUserName,
        IUserUniquenessChecker userChecker,
        IProfileUniquenessChecker profileChecker,
        [NotNull] IPasswordHelper passwordHelper,
        IHashGenerator hashGenerator,
        CancellationToken ct)
    {
        await VerifyEmailAsync(email, userChecker, ct).ConfigureAwait(false);

        string passwordHash = passwordHelper.HashPassword(password, out byte[]? passwordSalt);
        User user = new(email, passwordHash, passwordSalt, roles);

        var profile = await PublicProfile.CreateAsync(profileUserName.ToLowerInvariant(), ProfileType.Fan, profileChecker, user.Id, ct).ConfigureAwait(false);
        user.Profile = profile;
        user.SongLimitToUpload = 0;

        var favouritesPlaylist = await Playlist.NewFavouritesAsync(user, hashGenerator, ct);

        user.MusicSets.Add(favouritesPlaylist);

        user.AddDomainEvent(new UserCreatedEvent(email));

        return user;
    }

    public static async Task VerifyEmailAsync(string email, IUserUniquenessChecker checker, CancellationToken ct)
    {
        EnsureRule(new EmailRule(email));
        await EnsureRuleAsync(new UserEmailUniquenessRule(email, checker), ct).ConfigureAwait(false);
    }

    public bool HasActiveSubscriptionOfType(string subscriptionType)
    {
        return Subscriptions.Any(x =>
            x.Subscription.SubscriptionType.ToLowerInvariant() == subscriptionType.ToLowerInvariant()
            && x.IsActive);
    }

    public SecretToken? GetTokenOfValue(string secretToken)
    {
        return SecretTokens.FirstOrDefault(x => x.Token == secretToken);
    }

    public IEnumerable<string> ActiveSubscriptionTypes
        => Subscriptions.Where(x => x.IsActive).Select(x => x.Subscription.SubscriptionType);

    public UserSubscription? GetIncompleteSubscriptionOfType(string subscriptionType)
    {
        return Subscriptions.FirstOrDefault(x =>
            x.Subscription.SubscriptionType.ToLowerInvariant() == subscriptionType.ToLowerInvariant()
            && !x.CheckoutSessionIsProcessed
            && !string.IsNullOrWhiteSpace(x.CheckoutSessionId));
    }

    public short? GetTrialDaysIfEntitled(string subscriptionType)
    {
        var previousSubscription = Subscriptions.FirstOrDefault(x =>
            x.Subscription.SubscriptionType.ToLowerInvariant() == subscriptionType.ToLowerInvariant()
            && x.CheckoutSessionIsProcessed);
        return previousSubscription is not null ? previousSubscription.Subscription.FreeTrialInDays : null;
    }

    public bool VerifyPassword(string passwordToVerify, [NotNull] IPasswordHelper passwordHelper)
    {
        return passwordHelper.VerifyPassword(passwordToVerify, PasswordHash, PasswordSalt);
    }

    public void SaveMusicSet(MusicSet collection)
    {
        EnsureRule(new SavedMusicSetRule(this, collection));

        SavedMusicSets.Add(SavedMusicSet.New(this, collection));
    }

    public void UpdatePassword(string newPassword, [NotNull] IPasswordHelper passwordHelper)
    {
        EnsureRule(new NewPasswordRule(newPassword, this, passwordHelper));

        string passwordHash = passwordHelper.HashPassword(newPassword, out byte[]? passwordSalt);

        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;

        RevokeRefreshTokens();

        AddDomainEvent(new PasswordUpdatedEvent(Id));
    }

    public async Task UpdateUsernameAsync(string username, IProfileUniquenessChecker profileUniquenessChecker, CancellationToken ct)
    {
        await Profile!.UpdateUsernameAsync(username, profileUniquenessChecker, ct);
    }

    public void UpdateName(string name)
    {
        Profile!.UpdateName(name);
    }

    public void UpdateBio(string bio)
    {
        Profile!.UpdateBio(bio);
    }

    public void UpdateProfileType(ProfileType profileType)
    {
        EnsureRule(new UpdateProfileTypeRule(this));
        
        Profile.SetProfileType(profileType);
        SetSongLimit(profileType);
    }

    public void SetCustomerStripeId(string value) => CustomerStripeId = value;

    public void RevokeRefreshTokens()
    {
        foreach (var token in SecretTokens.Where(x => x.TokenType is SecretTokenType.Refresh).ToList())
        {
            token.RevokeToken();
        }
    }

    public IReadOnlyCollection<string> SplitRoles => new ReadOnlyCollection<string>(Roles.Split(','));

    private void SetSongLimit(ProfileType profileType)
    {
        SongLimitToUpload = profileType switch
        {
            ProfileType.Artist => 30,
            ProfileType.Label => 300,
            ProfileType.Fan => 0,
            ProfileType.Editor => 0,
            _ => 0
        };
    }

    private User() { }
}
