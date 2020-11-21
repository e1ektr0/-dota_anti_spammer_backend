namespace DotaAntiSpammerCommon.Models
{
    public class WardPlaced
    {
        public bool Obs { get; set; }
        public int Time { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double VecX { get; set; }
        public double VecY { get; set; }
        public bool Mine { get; set; }
        public long MatchId { get; set; }
    }
}