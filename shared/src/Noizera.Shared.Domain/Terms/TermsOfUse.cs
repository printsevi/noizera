using Noizera.Shared.Domain.Common;

namespace Noizera.Shared.Domain.Terms;

public class TermsOfUse : Entity
{
    public DateTimeOffset EffectiveDate { get; set; }
    public string Content { get; set; } = null!;

    public TermsOfUse() : base()
    { }
}
