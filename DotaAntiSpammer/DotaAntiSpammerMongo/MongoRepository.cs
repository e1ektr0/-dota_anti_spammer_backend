using System.Collections.Generic;
using System.Linq;
using DotaAntiSpammerCommon;
using DotaAntiSpammerMongo.Models;
using DotaAntiSpammerMongo.Models.Match;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Player = DotaAntiSpammerCommon.Models.Player;

namespace DotaAntiSpammerMongo
{
    public class MongoRepository
    {
        private const string MatchesCollectionName = "matches";
        private const string ConfigCollectionName = "config";
        private const string ResultsCollectionName = "results";
        private const string PlayerAccountsCollectionName = "player_accounts";
        private readonly IMongoDatabase _database;
        private readonly Config _config;
        private bool _baseAccountsListLoaded;

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
            var stringFilter = "{ account_id: { $in: [" + accounts + "] } }";
            return collection.Find(stringFilter).ToList().Select(n => BsonSerializer.Deserialize<PlayerResult>(n))
                .ToList();
        }

        public List<PlayerResult> GetResultsByHeroId(in int heroId)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ hero_id: " + heroId + " }";
            var resultsByHeroId = collection.Find(stringFilter).ToList()
                .Select(n => BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
            return resultsByHeroId;
        }


        public List<PlayerResult> GetResultsByAccountId(in long accountId, in int heroId)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ account_id: " + accountId + ",hero_id: " + heroId + " }";
            var resultsByHeroId = collection.Find(stringFilter).ToList()
                .Select(n => BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
            return resultsByHeroId;
        }

        public List<PlayerResult> GetResultsByAccountId(in long accountId)
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var stringFilter = "{ account_id: " + accountId + "}";
            var resultsByHeroId = collection.Find(stringFilter).ToList()
                .Select(n => BsonSerializer.Deserialize<PlayerResult>(n)).ToList();
            return resultsByHeroId;
        }

        public List<long> GetAllAccountId()
        {
            var collection = _database.GetCollection<BsonDocument>(ResultsCollectionName);
            var accList = collection.Distinct<long>("account_id", new BsonDocument()).ToList();
            return accList;
        }

        public IEnumerable<PlayerAccountMongo> ChangedResults()
        {
            var collection = _database.GetCollection<BsonDocument>(PlayerAccountsCollectionName);
            var filter = "{ '$expr': { '$ne': ['$processed_match_seq_num', '$last_match_seq_num'] } } ";
            var list = collection.Find(filter)
                .ToList();
            var results = list.Select(n => BsonSerializer.Deserialize<PlayerAccountMongo>(n));
            return results;
        }

        public void UpdateResult(Player player, ulong max)
        {
            var collection = _database.GetCollection<BsonDocument>(PlayerAccountsCollectionName);
            var playerAccount = new PlayerAccount
            {
                account_id = player.AccountId,
                stats = player,
                processed_match_seq_num = max,
            };
            collection.UpdateOne(Builders<BsonDocument>.Filter.Eq("account_id", player.AccountId),
                new BsonDocument("$set", playerAccount.ToBsonDocument()), new UpdateOptions
                {
                    IsUpsert = true
                });
        }

        public List<PlayerAccountMongo> GetPlayers(string accounts)
        {
            var collection = _database.GetCollection<BsonDocument>(PlayerAccountsCollectionName);
            var stringFilter = "{ account_id: { $in: [" + accounts + "] } }";
            var resultsByHeroId = collection.Find(stringFilter).ToList()
                .Select(n => BsonSerializer.Deserialize<PlayerAccountMongo>(n)).ToList();
            return resultsByHeroId;
        }


        public bool HighRankMatch(string accounts)
        {
            var collection = _database.GetCollection<BsonDocument>(PlayerAccountsCollectionName);
            if (!_baseAccountsListLoaded)
            {
                var countDocumentsAsync = collection.CountDocuments(new BsonDocument());
                if (countDocumentsAsync > 50 * 1000)
                    _baseAccountsListLoaded = true;
            }

            if (!_baseAccountsListLoaded)
                return false;

            var stringFilter = "{ account_id: { $in: [" + accounts + "] } }";
            var profile = collection.Find(stringFilter).FirstOrDefault();
            return profile != null;
        }
    }
}