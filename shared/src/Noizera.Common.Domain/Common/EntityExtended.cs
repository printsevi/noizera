namespace Noizera.Common.Domain.Common;

public abstract class EntityExtended : Entity
{
    public string PublicId { get; private set; } = string.Empty;

    public abstract string PublicIdPrefix { get; }

    protected EntityExtended(string publicId) : base()
        => PublicId = $"{PublicIdPrefix}{publicId}".ToUpperInvariant();

    protected EntityExtended() { }
}