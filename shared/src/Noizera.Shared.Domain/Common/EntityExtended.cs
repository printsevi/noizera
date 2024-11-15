using System.Diagnostics.CodeAnalysis;

namespace Noizera.Common.Domain.Common;

public abstract class EntityExtended : Entity
{
    public string PublicId { get; private set; } = string.Empty;

    public abstract string PublicIdPrefix { get; }

    protected EntityExtended(string publicId) : base()
        => PublicId = $"{PublicIdPrefix}{publicId}".ToUpperInvariant();

    protected void UpdatePublicId([NotNull] string newPublicId) => PublicId = newPublicId.ToUpperInvariant();

    protected EntityExtended() { }
}