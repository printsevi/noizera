namespace Noizera.Shared.Persistence.Mongo;

public class MongoDbSettings
{
    public required string ConnectionString { get; set; }
    public required string DatabaseName { get; set; }
    public required int ChunkSizeBytes { get; set; }
}
