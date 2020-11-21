using System.Collections.Generic;
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

    public class WardResultsMongo
    {
        public ObjectId _id { get; set; }
        public long match_id { get; set; }
        public long account_id { get; set; }
        public int hero_id { get; set; }
        public bool radiant { get; set; }
        public List<WardResult> results { get; set; }
    }

    public class WardResult
    {
        public bool obs { get; set; }
        public bool mine { get; set; }
        public int time { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public double vecx { get; set; }
        public double vecy { get; set; }
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