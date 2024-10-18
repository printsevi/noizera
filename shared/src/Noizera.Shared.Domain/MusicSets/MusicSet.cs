using Microsoft.AspNetCore.Http;
using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.MusicCollectionCredits;
using Noizera.Shared.Domain.MusicCollectionSongs;
using Noizera.Shared.Domain.SavedMusicSets;
using Noizera.Shared.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.MusicSets;

public class MusicSet : EntityExtended, IDeletable
{
    public string Title { get; protected set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CoverImageMongoId { get; private set; }
    public string? CoverImageOriginalName { get; private set; }
    public DateOnly? ReleaseDate { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; } = null!;
    public bool AllowedAsPreview { get; private set; }
    public bool IsDeleted { get; set; }
    public ICollection<MusicCollectionSong> MusicCollectionSongs { get; } = [];
    public ICollection<MusicCollectionCredit> Credits { get; } = [];
    public ICollection<SavedMusicSet> UserLibraries { get; } = [];

    public override string PublicIdPrefix => "m_";

    protected MusicSet([NotNull] User user, string title = "") : base()
    {
        OwnerId = user.Id;
        Title = title;
    }

    public async Task UploadCoverAsync(Guid userId, ICoverImageUploader uploader, IFormFile file, CancellationToken ct)
    {
        ValidateOwner(userId);

        string mongoFileId = await uploader.UploadAsync(file, PublicId, ct).ConfigureAwait(false);

        SetCoverImageInformation(mongoFileId, ValidFileName.New(file.FileName));
    }

    public void DeleteCover(Guid userId)
    {
        ValidateOwner(userId);

        SetCoverImageInformation(string.Empty, ValidFileName.New(string.Empty));
    }

    private void SetCoverImageInformation(string coverImageMongoId, [NotNull] ValidFileName fileName)
    {
        CoverImageMongoId = coverImageMongoId;
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
        foreach (var song in MusicCollectionSongs.OrderBy(x => x.Sequence).ToList())
        {
            song.SetSequence(sequence);
            sequence++;
        }
    }

    public void SwitchSongs(Guid activeSongId, Guid overSongId, Guid userId)
    {
        ValidateOwner(userId);

        var activeSong = MusicCollectionSongs.FirstOrDefault(x => x.SongId == activeSongId);
        var overSong = MusicCollectionSongs.FirstOrDefault(x => x.SongId == overSongId);

        EnsureRule(new SequenceUpdatingRule(activeSong, overSong));

        short overSongSequence = overSong!.Sequence;
        overSong!.SetSequence(activeSong!.Sequence);
        activeSong.SetSequence(overSongSequence);
    }

    public void ValidateOwner(Guid userId) => EnsureRule(new MusicSetOwnerRule(userId, this));

    protected MusicSet() { }
}
