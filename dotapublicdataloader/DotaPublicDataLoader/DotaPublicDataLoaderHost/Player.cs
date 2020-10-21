using System;

namespace DotaPublicDataLoaderHost
{
    public class Player
    {
        public Int64 account_id { get; set; }
        public Byte player_slot { get; set; }
        public Byte hero_id { get; set; }
        public Byte hero_pick_order { get; set; }
    }
}