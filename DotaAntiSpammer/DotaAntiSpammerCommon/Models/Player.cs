using System.Collections.Generic;

namespace DotaAntiSpammerCommon.Models
{
    public class Player
    {
        public long AccountId { get; set; }
        public int TotalGames { get; set; }
        public decimal WinRate { get; set; }
        public List<Hero> Heroes { get; set; }
    }
}