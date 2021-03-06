﻿using System.Collections.Generic;
using MongoDB.Bson;

namespace DotaAntiSpammerMongo.Models
{
    public class MatchAnalyze
    {
        public ObjectId _id { get; set; }
        public ulong match_id { get; set; }
        public ulong replay_salt { get; set; }
        public int cluster { get; set; }
                
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
        public int? rank { get; set; }
        public LongObject party_id { get; set; }
        public List<long> party { get; set; }
    }
}