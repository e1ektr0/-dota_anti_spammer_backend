using MongoDB.Bson;

namespace DotaAntiSpammerMongo.Models
{
    public class Config
    {
        public ObjectId _id { get; set; }
        public ulong? max_seq { get; set; }
    }
}