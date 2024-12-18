using Noizera.Common.Domain.AlbumCredits;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Events;
using Noizera.Common.Domain.Users;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.MusicSets;

public sealed class Album : MusicSet
{
    public AlbumStatus AlbumStatus { get; private set; } = AlbumStatus.Draft;

    public DateOnly? AlbumReleaseDate { get; private set; }

    public ICollection<AlbumCredit> AlbumCredits { get; } = [];

    public override string PublicIdPrefix => "a-";

    public bool IsProcessable => AlbumStatus is AlbumStatus.Submitted;

    public bool IsEditable => AlbumStatus is AlbumStatus.Draft or AlbumStatus.Submitted;

    private Album(User user, string publicId) : base(user, publicId)
    {
    }

    public static async Task<Album> NewAsync(User user, [NotNull]IHashGenerator hashGenerator, CancellationToken ct)
    {
        EnsureRule(new AlbumCreationRule(user));

        string publicId = await hashGenerator.GenerateAsync(ct).ConfigureAwait(false);
        Album album = new(user, publicId);

        return album;
    }

    public void SetReleaseDate(DateOnly? date)
    {
        AlbumReleaseDate = date;
    }

    public void Submit(Guid userId)
    {
        ValidateOwner(userId);
        EnsureRule(new AlbumSubmissionRule(this));

        AlbumStatus = AlbumStatus.Submitted;

        AddDomainEvent(new AlbumSubmittedEvent(Id));
    }

    public void Release()
    {
        EnsureRule(new AlbumReleaseRule(this));

        AlbumStatus = AlbumStatus.Released;

        foreach (var song in MusicSetSongs)
        {
            song.Release();
        }

        AddDomainEvent(new AlbumReleasedEvent(Id));
    }

    public void ValidateDraft() => EnsureRule(new DraftAlbumRule(this));

    private Album() { }
}
