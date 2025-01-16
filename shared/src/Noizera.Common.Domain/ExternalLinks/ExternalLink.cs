using System.Diagnostics.CodeAnalysis;
using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Profiles;

namespace Noizera.Common.Domain.ExternalLinks;

public class ExternalLink : Entity
{
    public Uri? Url { get; private set; } = null!;
    public Guid ProfileId { get; private set; }

    public PublicProfile Profile { get; } = null!;

    private ExternalLink(Uri? url, PublicProfile profile) : base()
    {
        Url = url;
        ProfileId = profile.Id;
    }

    public void UpdateUrl(Uri? url)
    {
        Url = url;
    }

    public static ExternalLink New(Uri? url, [NotNull] PublicProfile profile)
    {
        ExternalLink result = new(url, profile);

        return result;
    }

    private ExternalLink() { }
}
