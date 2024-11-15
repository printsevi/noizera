using Noizera.Common.Domain.Common;

namespace Noizera.Common.Domain.Terms;

public class TermsOfUse : Entity
{
    public DateTimeOffset EffectiveDate { get; set; }
    public string Content { get; set; } = null!;

    public TermsOfUse() : base()
    { }
}
