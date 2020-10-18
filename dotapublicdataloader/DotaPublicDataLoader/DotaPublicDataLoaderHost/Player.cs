using System;
using System.Collections.Generic;

namespace DotaPublicDataLoaderHost
{
    public class Player
    {
        public IList<AbilityUpgrade> ability_upgrades { get; set; }
        public IList<AdditionalUnit> additional_units { get; set; }
        public Int64 account_id { get; set; }
        public Byte player_slot { get; set; }
        public Byte hero_id { get; set; }
        public UInt16 item_0 { get; set; }
        public UInt16 item_1 { get; set; }
        public UInt16 item_2 { get; set; }
        public UInt16 item_3 { get; set; }
        public UInt16 item_4 { get; set; }
        public UInt16 item_5 { get; set; }
        public Byte kills { get; set; }
        public Byte deaths { get; set; }
        public Byte assists { get; set; }
        public Byte leaver_status { get; set; }
        public UInt16 last_hits { get; set; }
        public UInt16 denies { get; set; }
        public Int16 gold_per_min { get; set; }
        public Int16 xp_per_min { get; set; }
        public SByte level { get; set; }
        public Int32 gold { get; set; }
        public Int32 gold_spent { get; set; }
        public Int32 hero_damage { get; set; }
        public Int32 tower_damage { get; set; }
        public Int32 hero_healing { get; set; }
    }
}