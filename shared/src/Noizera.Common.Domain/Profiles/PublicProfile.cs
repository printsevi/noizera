using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.ProfileRelations;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.Profiles;

public class PublicProfile : EntityExtended, IDeletable
{
    public string Name { get; private set; } = string.Empty;
    public ProfileType ProfileType { get; private set; }
    public string? Bio { get; private set; }
    public string? ImageS3Folder { get; private set; }
    public bool IsDeleted { get; set; }
    public Guid? UserId { get; private set; }

    public User? User { get; }
    public ICollection<AlbumCredit> AlbumCredits { get; } = [];
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
    }

    public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : PublicId;

    public static async Task<PublicProfile> CreateAsync(string username, ProfileType profileType, IProfileUniquenessChecker checker, Guid? userId = null, CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);

        PublicProfile result = new(username, username, userId);

        result.SetProfileType(profileType);

        return result;
    }

    public async Task UpdateUsernameAsync(string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);
        UpdatePublicId(username);
    }

    public void UpdateName(string name) => Name = string.IsNullOrWhiteSpace(name) ? PublicId : name;

    public void UpdateBio(string bio) => Bio = bio;

    public void SetProfileType(ProfileType profileType) => ProfileType = profileType;

    public static async Task VerifyUsernameAsync([NotNull] string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        EnsureRule(new ProfileUsernameRule(username));
        await EnsureRuleAsync(new ProfileUsernameUniquenessRule(username, checker), ct).ConfigureAwait(false);
    }

    private PublicProfile() { }
}
