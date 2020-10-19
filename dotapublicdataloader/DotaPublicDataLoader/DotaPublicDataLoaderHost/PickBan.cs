using System;

namespace DotaPublicDataLoaderHost
{
    public class PickBan
    {
        public bool is_pick { get; set; }
        public Byte hero_id { get; set; }
        public Byte team { get; set; }
        public Byte order { get; set; }
    }
}