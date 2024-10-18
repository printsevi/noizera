using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using Noizera.Shared.Contracts.Services;
using Noizera.Shared.Persistence.Mongo;
using System.Diagnostics.CodeAnalysis;

namespace Noizera.Infrastructure.CoverImages;

public class CoverImageMongoService(MongoDbContext db) : ICoverImageService
{
    public async Task<string> UploadImageAsync([NotNull] IFormFile file, string mongoFileName, CancellationToken ct)
    {
        GridFSUploadOptions options = new()
        {
            Metadata = new BsonDocument { { "fileType", "image/jpeg" } }
        };

        using var stream = await db.CoverImageBucketWritable.OpenUploadStreamAsync(mongoFileName, options, ct).ConfigureAwait(false);
        string mongoFileId = stream.Id.ToString();
        await file.CopyToAsync(stream, ct).ConfigureAwait(false);
        await stream.CloseAsync(ct).ConfigureAwait(false);

        return mongoFileId;
    }

    public async Task<Stream> GetImageFileAsStreamAsync(string mongoFileName, CancellationToken ct)
    {
        GridFSDownloadByNameOptions options = new()
        {
            Seekable = false
        };

        return await db.CoverImageBucketReadable.OpenDownloadStreamByNameAsync(mongoFileName, options, ct).ConfigureAwait(false);
    }
}
