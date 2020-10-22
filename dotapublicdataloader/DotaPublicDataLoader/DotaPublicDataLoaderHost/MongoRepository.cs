﻿using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DotaPublicDataLoaderHost
{
    public class Config
    {
        public ObjectId _id { get; set; }
        public ulong? max_seq { get; set; }
    }

    public class MongoRepository
    {
        private readonly IMongoDatabase _database;
        private Config _config;
        private const string MatchesCollectionName = "matches";
        private const string ConfigCollectionName = "config";

        public MongoRepository()
        {
            var str = "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false";
            var client = new MongoClient(str);

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

            if (listCollections.All(n => n["name"] != ConfigCollectionName))
            {
                _database.CreateCollection(ConfigCollectionName);
                GetConfigCollection().InsertOne(new Config().ToBsonDocument());
            }
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
    }
}