using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Net.Http;

namespace Noizera.Shared.Persistence.Mongo;

public class MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
{
    private const string audioCollectionName = "audio";
    private const string coverImageCollectionName = "cover";
    private const string emailCollectionName = "email";

    private readonly MongoDbSettings settings = mongoDbSettings.Value;
    private readonly IMongoDatabase db = new MongoClient(mongoDbSettings.Value.ConnectionString).GetDatabase(mongoDbSettings.Value.DatabaseName);

    public IGridFSBucket AudioBucketWritable => new GridFSBucket(db, new GridFSBucketOptions
    {
        BucketName = audioCollectionName,
        ChunkSizeBytes = settings.ChunkSizeBytes,
        WriteConcern = WriteConcern.WMajority,
        ReadPreference = ReadPreference.Secondary
    });

    public IGridFSBucket AudioBucketReadable => new GridFSBucket(db, new GridFSBucketOptions
    {
        BucketName = audioCollectionName,
        ChunkSizeBytes = settings.ChunkSizeBytes,
        ReadPreference = ReadPreference.Primary
    });

    public IGridFSBucket CoverImageBucketWritable => new GridFSBucket(db, new GridFSBucketOptions
    {
        BucketName = coverImageCollectionName,
        ChunkSizeBytes = settings.ChunkSizeBytes,
        WriteConcern = WriteConcern.WMajority,
        ReadPreference = ReadPreference.Secondary
    });

    public IGridFSBucket CoverImageBucketReadable => new GridFSBucket(db, new GridFSBucketOptions
    {
        BucketName = coverImageCollectionName,
        ChunkSizeBytes = settings.ChunkSizeBytes,
        ReadPreference = ReadPreference.Primary
    });

    public async Task UpsertEmailTemplateAsync(string emailTemplateName, string content, CancellationToken ct = default)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("templateName", emailTemplateName);
        var document = await EmailCollection.Find(filter).FirstOrDefaultAsync();
        if (document is not null)
        {
            document["htmlContent"] = content;
            await EmailCollection.UpdateOneAsync(filter, document, new(), ct);
        } 
        else
        {
            document = new BsonDocument
            {
                { "templateName", emailTemplateName },
                { "htmlContent", content },
                { "createdAt", DateTime.UtcNow }
            };

            await EmailCollection.InsertOneAsync(document, new(), ct);
        }
    }

    public async Task<string?> GetEmailTemplateContent(string emailTemplateName, CancellationToken ct = default)
    {
        var document = await GetEmailTemplateByName(emailTemplateName, ct);

        return document?["htmlContent"]?.AsString;
    }

    private async Task<BsonDocument?> GetEmailTemplateByName(string emailTemplateName, CancellationToken ct)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("templateName", emailTemplateName);
        var document = await EmailCollection.Find(filter).FirstOrDefaultAsync(ct);

        return document;
    }

    private IMongoCollection<BsonDocument> EmailCollection => db.GetCollection<BsonDocument>(emailCollectionName);
}
