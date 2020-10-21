using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace DotaPublicDataLoaderHost
{
    public class MatchDetails
    {
        public IList<Player> players { get; set; }
        public bool radiant_win { get; set; }
        public UInt64 start_time { get; set; }
        public UInt64 match_id { get; set; }
        public UInt64 match_seq_num { get; set; }
        public Byte lobby_type { get; set; }
        public Byte game_mode { get; set; }
        public Int16 cluster { get; set; }
    }

    public class MatchDetailsMongo : MatchDetails
    {
        public ObjectId _id { get; set; }
        public string reserve_instance_id { get; set; }
        public long reserve_update_time { get; set; }
    }
}