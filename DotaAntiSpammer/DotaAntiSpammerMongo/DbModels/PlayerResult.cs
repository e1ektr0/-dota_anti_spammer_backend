using System.Collections.Generic;
using MongoDB.Bson;

namespace DotaAntiSpammerMongo.Models
{
    public class LongObject
    {
        public long low { get; set; }
        public long high { get; set; }
        public bool unsigned { get; set; }
    }
    public class PlayerResult
    {
        public ObjectId _id { get; set; }
        public long account_id { get; set; }
        public ulong startTime { get; set; }
        public byte hero_id { get; set; }
        public byte hero_pick_order { get; set; }
        public ulong match_id { get; set; }
        public ulong match_seq_num { get; set; }
        public bool win { get; set; }
        public string steam_id { get; set; }
        public List<int> bans { get; set; }
        public int rank { get; set; }
        public LongObject party_id { get; set; }
    }
}