namespace Noizera.Shared.Domain.Common;

public abstract class EntityExtended : Entity
{
    public string PublicId { get; protected set; } = string.Empty;

    public abstract string PublicIdPrefix { get; }

    public void SetPublicId(string value) => PublicId = $"{PublicIdPrefix}{value}";

    protected EntityExtended(string publicId = "") : base() => PublicId = publicId;

    protected EntityExtended() { }
}