namespace Noizera.Common.Domain.Common;

public interface IHashGenerator
{
    Task<string> GenerateAsync(CancellationToken ct);
}
