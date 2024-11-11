namespace Noizera.Shared.Domain.Common;

public abstract class EntityExtended : Entity
{
    public string PublicId { get; protected set; } = string.Empty;

    public abstract string PublicIdPrefix { get; }

    protected EntityExtended(string publicId) : base()
    {
        PublicId = $"{PublicIdPrefix}{publicId}";
    }

    protected EntityExtended() { }
}