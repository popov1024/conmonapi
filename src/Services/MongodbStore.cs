using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using conmonapi.Models;
using System.Linq;

namespace conmonapi.Services
{
    public class MongodbStore : IContentSore
    {
        MongoClient _client;
        IMongoDatabase _db;
        GridFSBucket _bucket;

        public MongodbStore(string connectionString, string database)
        {
            _client = new MongoClient(connectionString);

            _db = _client.GetDatabase(database);

            var isReplicaSet = _client.Cluster.Settings.ReplicaSetName != null;
            
            _bucket = new GridFSBucket(
                _db,
                new GridFSBucketOptions
                {
                    BucketName = "content",
                    ChunkSizeBytes = 1048576, // 1MB
                    WriteConcern = isReplicaSet ? WriteConcern.W2 : WriteConcern.W1, // Check write to a replica set
                    ReadPreference = ReadPreference.SecondaryPreferred // read from secondary
                }
            );
        }
        
        async Task<bool> IContentSore.DeleteContentAsync(Content content)
        {
            throw new NotImplementedException();
        }

        async Task<Tuple<byte[], Content>> IContentSore.GetContentAsync(Content content)
        {
            byte[] bytes;
            content.ContentType = await GetContentTypeAsync(content.Id);
            try
            {
                bytes = await _bucket.DownloadAsBytesByNameAsync(content.Id);
            }
            catch (Exception)
            {
                return new Tuple<byte[], Content>(null, content);
            }
            return new Tuple<byte[], Content>(bytes, content);
        }

        async Task<IEnumerable<Content>> IContentSore.GetContentsAsync(int skip, int limit)
        {
            throw new NotImplementedException();
        }

        async Task<bool> IContentSore.SetContentAsync(Stream stream, Content content)
        {
            var options = new GridFSUploadOptions
            {
                Metadata = content.ToBsonDocument()
            };

            var id = await _bucket.UploadFromStreamAsync(content.Id, stream, options);

            return true;
        }

        async Task<bool> IContentSore.UpdateContentAsync(Stream stream, Content content)
        {
            throw new NotImplementedException();
        }

        private async Task<GridFSFileInfo> getFileInfo(string filename)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, filename);
            var sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime);
            var options = new GridFSFindOptions
            {
                Limit = 1,
                Sort = sort
            };
            using (var cursor = await _bucket.FindAsync(filter, options))
            {
                var fs = (await cursor.ToListAsync());
                if (fs != null && fs.Count > 0)
                {
                    return fs.First();
                }
                return null;
            }
        }

        private async Task<string> GetContentTypeAsync(string filename)
        {
            var fileInfo = await getFileInfo(filename);
            if (fileInfo == null)
            {
                return null;
            }
            BsonValue contentType;
            fileInfo.Metadata.TryGetValue("ContentType", out contentType);
            return contentType.AsString;
        }
    }
}