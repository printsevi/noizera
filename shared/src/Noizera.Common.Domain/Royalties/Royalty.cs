using Noizera.Common.Domain.Common;
using Noizera.Common.Domain.Users;

namespace Noizera.Common.Domain.Royalties;

public class Royalty : Entity
{
    public Guid PayerId { get; private set; }
    public User Payer { get; } = null!;
    public Guid PayeeId { get; private set; }
    public User Payee { get; } = null!;
    public float Amount { get; private set; }

    private Royalty(
        Guid payerId,
        Guid payeeId,
        float amount) : base()
    {
        PayerId = payerId;
        PayeeId = payeeId;
        Amount = amount;
    }

    public static Royalty New(
        Guid payerId,
        Guid payeeId,
        float amount)
    {
        Royalty result = new(payerId, payeeId, amount);

        return result;
    }

    private Royalty() { }
}
