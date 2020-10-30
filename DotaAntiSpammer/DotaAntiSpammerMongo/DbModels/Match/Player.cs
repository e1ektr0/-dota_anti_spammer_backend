namespace DotaAntiSpammerMongo.Models.Match
{
    public class Player
    {
        public long account_id { get; set; }
        public byte player_slot { get; set; }
        public byte hero_id { get; set; }
        public byte hero_pick_order { get; set; }
    }
}