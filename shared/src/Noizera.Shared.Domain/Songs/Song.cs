using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Domain.MusicCollectionSongs;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.Royalties;
using Noizera.Shared.Domain.SongCredits;
using Noizera.Shared.Domain.Streams;
using Noizera.Shared.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Shared.Domain.Songs;

public sealed class Song : EntityExtended
{
    public string? Title { get; private set; }
    public string? OriginalFileName { get; private set; }
    public string? OriginalFileExtension { get; private set; }
    public string? OriginalContentType { get; private set; }
    public long? OriginalContentLength { get; private set; }
    public string? OriginalBucketName { get; private set; }
    public string? Mp3BucketName { get; private set; }
    public long? Mp3ContentLength { get; private set; }
    public string? FlacBucketName { get; private set; }
    public long? FlacContentLength { get; private set; }
    public double? DurationInSeconds { get; private set; }
    public float? Danceability { get; private set; }
    public float? Energy { get; private set; }
    public char? Key { get; private set; }
    public string? Scale { get; private set; }
    public float? BPM { get; private set; }
    public bool IsPublic { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; } = null!;
    public ICollection<MusicCollectionSong> MusicCollectionSongs { get; } = [];
    public ICollection<SongCredit> Credits { get; } = [];
    public ICollection<Royalty> AssignedRoyalties { get; } = [];
    public ICollection<ListeningHistory> ListeningHistories { get; } = [];
    public ICollection<StreamInfo> Streams { get; } = [];

    public override string PublicIdPrefix => "t-";

    public bool HasAudioAttached => FlacContentLength > 0 && Mp3ContentLength > 0;

    private Song(User user, Album album)
        : base() => OwnerId = user.Id;

    public static Song Create([NotNull] User user, [NotNull] Album album)
    {
        EnsureRule(new AlbumOwnerSongRule(user, album));
        EnsureRule(new DraftAlbumRule(album));
        EnsureRule(new OwnerSongAmountRule(user));

        Song result = new(user, album);
        short maxSequence = album.MusicCollectionSongs.Count > 0 ? album.MusicCollectionSongs.Max(x => x.Sequence) : (short)0;
        MusicCollectionSong musicCollectionSong = MusicCollectionSong.Create(result, album, ++maxSequence);
        result.MusicCollectionSongs.Add(musicCollectionSong);
        album.MusicCollectionSongs.Add(musicCollectionSong);
        return result;
    }

    public void UploadOriginalAudioFile([NotNull] ValidFileName fileName, string extension, long contentLength, string bucket)
    {
        OriginalFileName = fileName.Value;
        OriginalFileExtension = extension;
        OriginalContentLength = contentLength;
    }

    public void DeleteOriginalAudioFile()
    {
        OriginalFileName = null;
        OriginalFileExtension = null;
        OriginalContentLength = null;
    }

    public void SaveAudioFileToMp3Bucket(string bucketName, long contentLength)
    {
        Mp3BucketName = bucketName;
        Mp3ContentLength = contentLength;
    }

    public void SaveAudioFileToFlacBucket(string bucketName, long contentLength, double duration)
    {
        FlacBucketName = bucketName;
        FlacContentLength = contentLength;
        DurationInSeconds = duration;
    }

    public void SetTitle(string newTitle, Album album, Guid userId)
    {
        ValidateOwner(userId);
        EnsureRule(new DraftAlbumRule(album));

        Title = newTitle;
    }

    public void ValidateOwner(Guid userId) => EnsureRule(new OwnerSongRule(this, userId));

    private Song() { }
}
