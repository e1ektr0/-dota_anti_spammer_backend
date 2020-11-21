using System.Collections.Generic;

namespace DotaAntiSpammerCommon.Models
{
    public class PlayerWards
    {
        public long AccountId { get; set; }
        
        public List<WardPlaced> Wards { get; set; }
        public int HeroId { get; set; }
    }
}