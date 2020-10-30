using DotaAntiSpammerCommon.Models;
using MongoDB.Bson;

namespace DotaAntiSpammerMongo.Models
{
    public class PlayerAccount
    {
        public long account_id { get; set; }
        public Player stats { get; set; }
        public ulong processed_match_seq_num { get; set; }
    }


    public class PlayerAccountMongo
    {
        public ObjectId _id { get; set; }
        public long account_id { get; set; }
        public Player stats { get; set; }
        public ulong processed_match_seq_num { get; set; }
        public ulong last_match_seq_num { get; set; }
    }
}