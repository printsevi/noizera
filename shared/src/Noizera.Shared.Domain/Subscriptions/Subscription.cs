using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.UserSubscriptions;
using System.Collections.ObjectModel;

namespace Noizera.Common.Domain.Subscriptions;

public class Subscription : Entity
{
    public string Title { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsDisabled { get; set; }
    public string ProfileTypes { get; set; } = null!;
    public string StripePriceId { get; set; } = null!;
    public short? FreeTrialInDays { get; set; }
    public bool IsAnnual { get; set; }
    public float RoyaltyShare { get; set; }
    public string SubscriptionType { get; set; } = null!;
    public ICollection<UserSubscription> UserSubscriptions { get; } = [];

    public ReadOnlyCollection<string> SplitProfileTypes => new(ProfileTypes.Split(','));

    public bool IsDeleting { get; set; }

    public Subscription() : base()
    { }
}
