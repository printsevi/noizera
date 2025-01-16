using Microsoft.AspNetCore.Http;
using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.ExternalLinks;
using Noizera.Common.Domain.ProfileRelations;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Noizera.Common.Domain.Profiles;

public class PublicProfile : EntityExtended, IDeletable
{
    public string Username { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ProfileType ProfileType { get; private set; }
    public string? Bio { get; private set; }
    public string? ImageS3Folder { get; private set; }
    public string? ImageOriginalName { get; private set; }
    public bool IsDeleted { get; set; }
    public Guid? UserId { get; private set; }

    public User? User { get; }
    public ICollection<AlbumCredit> AlbumCredits { get; } = [];
    public ICollection<ProfileRelation> Followers { get; } = [];
    public ICollection<ProfileRelation> Followings { get; } = [];
    public ICollection<ExternalLink> ExternalLinks { get; } = [];

    public override string PublicIdPrefix => "u-";

    private PublicProfile(
        string name,
        string username,
        Guid userId,
        string publicId)
            : base(publicId)
    {
        ProfileType = ProfileType.Fan;
        Username = username.ToUpperInvariant();
        Name = name;
        UserId = userId;
    }

    public string DisplayName => !string.IsNullOrWhiteSpace(Name) ? Name : Username;

    public static async Task<PublicProfile> CreateAsync(
        string username,
        ProfileType profileType,
        IProfileUniquenessChecker checker,
        [NotNull] IHashGenerator hashGenerator,
        Guid userId,
        CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);

        string publicId = await hashGenerator.GenerateAsync(ct).ConfigureAwait(false);

        PublicProfile result = new(username, username, userId, publicId);

        result.SetProfileType(profileType);

        return result;
    }

    public async Task UpdateUsernameAsync(string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        await VerifyUsernameAsync(username, checker, ct).ConfigureAwait(false);
        Username = username.ToUpperInvariant();
    }

    public void UpdateName(string name) => Name = string.IsNullOrWhiteSpace(name) ? Username : Regex.Replace(name.Trim(), @"\s+", " ");

    public void UpdateBio(string bio) => Bio = string.IsNullOrWhiteSpace(bio) ? string.Empty : Regex.Replace(bio.Trim(), @"\s+", " ");

    public void SetProfileType(ProfileType profileType) => ProfileType = profileType;

    public async Task UploadProfileImageAsync([NotNull] IProfileImageUploader uploader, IFormFile file, CancellationToken ct)
    {
        (_, string? s3Folder) = await uploader.UploadAsync(file, PublicId, ct).ConfigureAwait(false);

        SetProfileImageInformation(s3Folder, ValidFileName.New(file.FileName));
    }

    public void DeleteProfileImage()
    {
        ImageS3Folder = null;
        ImageOriginalName = null;
    }

    private void SetProfileImageInformation(string s3Folder, [NotNull] ValidFileName fileName)
    {
        ImageS3Folder = s3Folder;
        ImageOriginalName = fileName.Value;
    }

    public static async Task VerifyUsernameAsync([NotNull] string username, IProfileUniquenessChecker checker, CancellationToken ct = default)
    {
        EnsureRule(new ProfileUsernameRule(username));
        await EnsureRuleAsync(new ProfileUsernameUniquenessRule(username, checker), ct).ConfigureAwait(false);
    }

    private PublicProfile() { }
}
