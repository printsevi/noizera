namespace Noizera.Shared.Domain.Common;

public interface IHashGenerator
{
    Task<string> GenerateAsync(CancellationToken ct);
}
