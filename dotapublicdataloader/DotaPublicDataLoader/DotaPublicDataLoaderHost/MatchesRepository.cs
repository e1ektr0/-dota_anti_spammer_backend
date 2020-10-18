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
                    "mongodb+srv://e1ektr0:Tn7O5zyFdgZ0t3nP@cluster0.ncrvw.gcp.mongodb.net/dota2?retryWrites=true&w=majority");
            _database = client.GetDatabase("dota2");
            Ini();
        }

        private void Ini()
        {
            var listCollections = _database.ListCollections().ToList();
            if (listCollections.All(n => n["name"] != CollectionName))
            {
                _database.CreateCollection(CollectionName);
                //todo: create sorting index
            }
        }

        public ulong GetLastSeq()
        {
            var collection = GetCollection();
            var firstOrDefault = collection.Find(new BsonDocument())
                .Sort(Builders<BsonDocument>.Sort.Descending("match_seq_num"))
                .FirstOrDefault();
            if (firstOrDefault == null) 
                return 4756161920;
            var json = firstOrDefault.ToJson();
            var matchDetails = BsonSerializer.Deserialize<MatchDetailsMongo>(firstOrDefault);
            return matchDetails.match_seq_num;
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