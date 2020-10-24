using System.Collections.Generic;
using System.Linq;
using DotaAntiSpammerCommon;
using DotaAntiSpammerMongo.Models;
using DotaAntiSpammerMongo.Models.Match;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DotaAntiSpammerMongo
{
    public class MongoRepository
    {
        private const string MatchesCollectionName = "matches";
        private const string ConfigCollectionName = "config";
        private const string ResultsCollectionName = "results";
        private readonly IMongoDatabase _database;
        private readonly Config _config;

        public MongoRepository()
        {
            var client = new MongoClient(GlobalConfig.ConnectionString);

            _database = client.GetDatabase("dota2");
            Ini();
            var bsonDocument = GetConfigCollection().Find(new BsonDocument()).First();
            _config = BsonSerializer.Deserialize<Config>(bsonDocument);
        }

        private void Ini()
        {
            var listCollections = _database.ListCollections().ToList();
            if (listCollections.All(n => n["name"] != MatchesCollectionName))
            {
                _database.CreateCollection(MatchesCollectionName);
                var mongoCollection = _database.GetCollection<BsonDocument>(MatchesCollectionName);
                var indexJson = @"{'match_seq_num_1': -1}";
                mongoCollection.Indexes.CreateOne(
                    new CreateIndexModel<BsonDocument>(new JsonIndexKeysDefinition<BsonDocument>(indexJson)));
            }

            if (listCollections.Any(n => n["name"] == ConfigCollectionName)) 
                return;
            
            _database.CreateCollection(ConfigCollectionName);
            GetConfigCollection().InsertOne(new Config().ToBsonDocument());
        }

        public ulong? GetLastSeq()
        {
            return _config.max_seq;
        }

        public void Insert(IList<MatchDetails> matches)
        {
            var collection = GetCollection();
            var documents = matches.Select(n => n.ToBsonDocument()).ToList();
            collection.InsertMany(documents);
        }

        private IMongoCollection<BsonDocument> GetCollection()
        {
            var collection = _database.GetCollection<BsonDocument>(MatchesCollectionName);
            return collection;
        }

        private IMongoCollection<BsonDocument> GetConfigCollection()
        {
            var collection = _database.GetCollection<BsonDocument>(ConfigCollectionName);
            return collection;
        }

        public void UpdateMaxSeq(ulong? seq)
        {
            var configCollection = GetConfigCollection();
            _config.max_seq = seq;
            configCollection.UpdateOne(Builders<BsonDocument>.Filter.Eq("_id", _config._id),
                Builders<BsonDocument>.Update.Set("max_seq", seq));
        }

        public List<PlayerResult> GetResults(string accounts)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ account_id: { $in: ["+accounts+"] } }";
            return collection.Find(stringFilter).ToList().Select(n=> BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
        }

        public List<PlayerResult> GetResultsByHeroId(in int heroId)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ hero_id: "+heroId+" }";
            var resultsByHeroId = collection.Find(stringFilter).ToList().Select(n=> BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
            return resultsByHeroId;
        }
        
        
        public  List<PlayerResult> GetResultsByAccountId(in int accountId, in int heroId)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ account_id: "+accountId+",hero_id: "+heroId+" }";
            var resultsByHeroId = collection.Find(stringFilter).ToList().Select(n=> BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
            return resultsByHeroId;
        }
    }
}