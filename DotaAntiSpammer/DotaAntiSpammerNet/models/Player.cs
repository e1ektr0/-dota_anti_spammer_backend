using System.Collections.Generic;

namespace DotaAntiSpammerNet.models
{
    public class Player
    {
        public List<Hero> Heroes { get; set; }
        public int TotalGames { get; set; }
        public decimal WinRate { get; set; }
    }
}