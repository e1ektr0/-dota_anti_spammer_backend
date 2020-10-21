using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DotaPublicDataLoaderHost
{
    public class MatchesRepository
    {
        private readonly IMongoDatabase _database;
        private const string CollectionName = "matches";

        public MatchesRepository()
        {
            var client =
                new MongoClient(
                    "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false");
            _database = client.GetDatabase("dota2");
            Ini();
        }

        private void Ini()
        {
            var listCollections = _database.ListCollections().ToList();
            if (listCollections.All(n => n["name"] != CollectionName))
            {
                _database.CreateCollection(CollectionName);
                var mongoCollection = _database.GetCollection<BsonDocument>(CollectionName);
                var indexJson = @"{'match_seq_num_1': -1}";
                mongoCollection.Indexes.CreateOne(new CreateIndexModel<BsonDocument>(new JsonIndexKeysDefinition<BsonDocument>(indexJson)));
            }
        }

        public ulong? GetLastSeq()
        {
            var collection = GetCollection();
            var firstOrDefault = collection.Find(new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Descending("match_seq_num"))
                .FirstOrDefault();
            if (firstOrDefault == null)     
                return null;
            var json = firstOrDefault.ToJson();
            var matchDetails = BsonSerializer.Deserialize<MatchDetailsMongo>(firstOrDefault);
            return matchDetails.match_seq_num;
        }
        public MatchDetailsMongo GetLastMatch()
        {
            var collection = GetCollection();
            var firstOrDefault = collection.Find(new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Descending("match_seq_num"))
                .FirstOrDefault();
            if (firstOrDefault == null)     
                return null;
            var json = firstOrDefault.ToJson();
            var matchDetails = BsonSerializer.Deserialize<MatchDetailsMongo>(firstOrDefault);
            return matchDetails;
        }

        public void Insert(IList<MatchDetails> matches)
        {
            var collection = GetCollection();
            var documents = matches.Select(n => n.ToBsonDocument()).ToList();
            collection.InsertMany(documents);
        }

        private IMongoCollection<BsonDocument> GetCollection()
        {
            var collection = _database.GetCollection<BsonDocument>(CollectionName);
            return collection;
        }
    }
}