namespace DotaAntiSpammerMongo.Models.Match
{
    public class PickBan
    {
        public bool is_pick { get; set; }
        public byte hero_id { get; set; }
        public byte team { get; set; }
        public byte order { get; set; }
    }
}