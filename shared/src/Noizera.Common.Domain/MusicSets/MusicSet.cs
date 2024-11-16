using Microsoft.AspNetCore.Http;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.MusicSetSongs;
using Noizera.Common.Domain.SavedMusicSets;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.MusicSets;

public class MusicSet : EntityExtended, IDeletable
{
    public string Title { get; protected set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CoverImageS3Folder { get; private set; }
    public long? CoverImageContentLength { get; private set; }
    public string? CoverImageOriginalName { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool AllowedAsPreview { get; private set; }
    public bool IsDeleted { get; set; }

    public User Owner { get; } = null!;
    public ICollection<MusicSetSong> MusicSetSongs { get; } = [];
    public ICollection<SavedMusicSet> UserLibraries { get; } = [];

    public override string PublicIdPrefix => "m_";

    protected MusicSet([NotNull] User user, string publicId, string title = "") : base(publicId)
    {
        OwnerId = user.Id;
        Title = title;
    }

    public async Task UploadCoverAsync(Guid userId, [NotNull] ICoverImageUploader uploader, IFormFile file, CancellationToken ct)
    {
        ValidateOwner(userId);

        (long contentLength, string? s3Folder) = await uploader.UploadAsync(file, PublicId, ct).ConfigureAwait(false);

        SetCoverImageInformation(contentLength, s3Folder, ValidFileName.New(file.FileName));
    }

    public void DeleteCover(Guid userId)
    {
        ValidateOwner(userId);

        SetCoverImageInformation(null, string.Empty, ValidFileName.New(string.Empty));
    }

    private void SetCoverImageInformation(long? contentLength, string s3Folder, [NotNull] ValidFileName fileName)
    {
        CoverImageContentLength = contentLength;
        CoverImageS3Folder = s3Folder;
        CoverImageOriginalName = fileName.Value;
    }

    public void SetTitle(string newTitle, Guid userId)
    {
        ValidateOwner(userId);

        Title = newTitle;
    }

    public void SetDescription(string newDescription, Guid userId)
    {
        ValidateOwner(userId);

        Description = newDescription;
    }

    public void FixSongSequences()
    {
        short sequence = 1;
        foreach (var song in MusicSetSongs.OrderBy(x => x.Sequence).ToList())
        {
            song.SetSequence(sequence);
            sequence++;
        }
    }

    public void SwitchSongs(Guid activeSongId, Guid overSongId, Guid userId)
    {
        ValidateOwner(userId);

        var activeSong = MusicSetSongs.FirstOrDefault(x => x.SongId == activeSongId);
        var overSong = MusicSetSongs.FirstOrDefault(x => x.SongId == overSongId);

        EnsureRule(new SequenceUpdatingRule(activeSong, overSong));

        short overSongSequence = overSong!.Sequence;
        overSong!.SetSequence(activeSong!.Sequence);
        activeSong.SetSequence(overSongSequence);
    }

    public void ValidateOwner(Guid userId) => EnsureRule(new MusicSetOwnerRule(userId, this));

    protected MusicSet() { }
}
