namespace DotaAntiSpammerCommon.Models
{
    public class PlayerPick
    {
        public long AccountId { get; set; }
        public int HeroId { get; set; }
        public bool Radiant { get; set; }
    }
}