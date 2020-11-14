using System.Collections.Generic;
using MongoDB.Bson;

namespace DotaAntiSpammerMongo.Models.Match
{
    public class MatchDetails
    {
        public ulong durtaion { get; set; }
        public int lobby_type { get; set; }
        public IList<Player> players { get; set; }
        public bool radiant_win { get; set; }
        public ulong start_time { get; set; }
        public ulong match_id { get; set; }
        public ulong match_seq_num { get; set; }
        public byte game_mode { get; set; }
        public short cluster { get; set; }
        public bool HightRankProof { get; set; }
        public bool NeedRankCheck { get; set; }
    }

    public class MatchDetailsMongo : MatchDetails
    {
        public ObjectId _id { get; set; }
        public string reserve_instance_id { get; set; }
        public long reserve_update_time { get; set; }
    }
}