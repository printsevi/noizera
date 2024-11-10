using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.ProfileRelations;
using Noizera.Shared.Domain.SongCredits;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.Profiles;

public class PublicProfile : EntityExtended, IDeletable
{
    public string Name { get; private set; } = string.Empty;
    public ProfileType ProfileType { get; private set; }
    public string? Bio { get; private set; } = null!;
    public short SongLimitToUpload { get; private set; }
    public bool IsDeleted { get; set; }
    public Guid? UserId { get; private set; }
    public User? User { get; }
    public ICollection<SongCredit> SongCredits { get; } = [];
    public ICollection<ProfileRelation> Followers { get; } = [];
    public ICollection<ProfileRelation> Followings { get; } = [];

    public override string PublicIdPrefix => "";

    private PublicProfile(
        string name,
        string username,
        Guid? userId = null)
            : base(username)
    {
        ProfileType = ProfileType.Fan;
        Name = name;
        UserId = userId;
        SongLimitToUpload = 0;
    }

    public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : PublicId;

    public static async Task<PublicProfile> CreateAsync(string username, ProfileType profileType, IProfileUniquenessChecker checker, Guid? userId = null, CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);

        var result = new PublicProfile(username, username, userId);

        result.SetProfileType(profileType);

        return result;
    }

    public async Task UpdateUsernameAsync(string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);
        PublicId = username;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateBio(string bio)
    {
        Bio = bio;
    }

    public void SetProfileType(ProfileType profileType)
    {
        //Check if it's eligable to update (Fan -> Artist)

        ProfileType = profileType;

        SetSongLimit(profileType);
    }

    public static async Task VerifyUsernameAsync(string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        username = username.ToLowerInvariant();
        EnsureRule(new ProfileUsernameRule(username));
        await EnsureRuleAsync(new ProfileUsernameUniquenessRule(username, checker), ct).ConfigureAwait(false);
    }

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

    private PublicProfile() { }
}
