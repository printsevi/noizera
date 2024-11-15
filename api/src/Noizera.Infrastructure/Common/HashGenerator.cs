using HashidsNet;
using Noizera.Common.Domain.Common;
using Noizera.Common.Persistence.SQL;

namespace Noizera.Infrastructure.Common;

public class HashGenerator(AppDbContext db) : IHashGenerator
{
    public async Task<string> GenerateAsync(CancellationToken ct)
    {
        Hashids hashids = new("CDB581A849B94AC899BB0ACDE57286B9");
        long numberId = await db.GetNextNumberIdSequenceValueAsync(ct).ConfigureAwait(false);
        string result = hashids.EncodeLong(numberId).ToUpperInvariant();

        return result;
    }
}
