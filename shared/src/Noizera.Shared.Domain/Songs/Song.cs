using Noizera.Shared.Domain.Common;
using Noizera.Shared.Domain.ListeningHistories;
using Noizera.Shared.Domain.MusicSets;
using Noizera.Shared.Domain.MusicSetSongs;
using Noizera.Shared.Domain.Royalties;
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
    public string? OriginalS3Folder { get; private set; }
    public string? MpegS3Folder { get; private set; }
    public long? MpegContentLength { get; private set; }
    public string? FlacS3Folder { get; private set; }
    public long? FlacContentLength { get; private set; }
    public double? DurationInSeconds { get; private set; }
    public float? Danceability { get; private set; }
    public float? Energy { get; private set; }
    public char? Key { get; private set; }
    public string? Scale { get; private set; }
    public float? BPM { get; private set; }
    public bool IsPublic { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid AlbumId { get; private set; }

    public Album Album { get; } = null!;
    public User Owner { get; } = null!;
    public ICollection<MusicSetSong> MusicSetSongs { get; } = [];
    public ICollection<Royalty> AssignedRoyalties { get; } = [];
    public ICollection<ListeningHistory> ListeningHistories { get; } = [];
    public ICollection<StreamInfo> Streams { get; } = [];

    public override string PublicIdPrefix => "t_";

    public bool HasAudioAttached => FlacContentLength > 0 && MpegContentLength > 0;

    private Song(User user, Album album, string publicId)
        : base(publicId)
    {
        OwnerId = user.Id;
        AlbumId = album.Id;
    }

    public static async Task<Song> NewAsync([NotNull] User user, [NotNull] Album album, IHashGenerator hashGenerator, CancellationToken ct)
    {
        EnsureRule(new AlbumOwnerSongRule(user, album));
        EnsureRule(new DraftAlbumRule(album));
        EnsureRule(new OwnerSongAmountRule(user));

        var publicId = await hashGenerator.GenerateAsync(ct);
        Song result = new(user, album, publicId);
        short maxSequence = album.MusicSetSongs.Count > 0 ? album.MusicSetSongs.Max(x => x.Sequence) : (short)0;
        MusicSetSong MusicSetSong = MusicSetSong.Create(result, album, ++maxSequence);
        result.MusicSetSongs.Add(MusicSetSong);
        album.MusicSetSongs.Add(MusicSetSong);
        return result;
    }

    public void UploadOriginalAudioFile([NotNull] ValidFileName fileName, string extension, long contentLength, string contentType, string bucket)
    {
        OriginalFileName = fileName.Value;
        OriginalFileExtension = extension;
        OriginalContentLength = contentLength;
        OriginalContentType = contentType;
        OriginalS3Folder = bucket;
    }

    public void DeleteOriginalAudioFile()
    {
        OriginalFileName = null;
        OriginalFileExtension = null;
        OriginalContentLength = null;
        OriginalContentType = null;
        OriginalS3Folder = null;
    }

    public void SaveAudioFileToMp3Bucket(string bucketName, long contentLength, double duration)
    {
        MpegS3Folder = bucketName;
        MpegContentLength = contentLength;
        DurationInSeconds = duration;
    }

    public void SaveAudioFileToFlacBucket(string bucketName, long contentLength)
    {
        FlacS3Folder = bucketName;
        FlacContentLength = contentLength;
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
