using Noizera.Shared.Domain.Events;
using Noizera.Shared.Domain.Users;

namespace Noizera.Shared.Domain.MusicSets;

public sealed class Album : MusicSet
{
    public AlbumStatus AlbumStatus { get; private set; } = AlbumStatus.Draft;

    public DateOnly? AlbumReleaseDate { get; private set; }


    public override string PublicIdPrefix => "a_";

    public bool IsProcessable => AlbumStatus is AlbumStatus.Submitted;

    public bool IsEditable => AlbumStatus is AlbumStatus.Draft or AlbumStatus.Submitted;

    private Album(User user) : base(user)
    {
    }

    public static Album Create(User user)
    {
        EnsureRule(new AlbumCreationRule(user));
        Album album = new(user);

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

        AddDomainEvent(new AlbumReleasedEvent(Id));
    }

    public void ValidateDraft() => EnsureRule(new DraftAlbumRule(this));

    private Album() { }
}
