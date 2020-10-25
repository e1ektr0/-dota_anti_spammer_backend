using System.Collections.Generic;
using DotaAntiSpammerNet.Controls.Player;

namespace DotaAntiSpammerNet.Controls.Match
{
    public partial class MatchMedium
    {
        public MatchMedium()
        {
            InitializeComponent();
        }

        public void Ini(DotaAntiSpammerCommon.Models.Match match)
        {
            if(match?.Players == null)
                return;
            var players = new List<PlayerMedium>
            {
                Player01,
                Player02,
                Player03,
                Player04,
                Player05,
                Player11,
                Player12,
                Player13,
                Player14,
                Player15
            };
            for (var i = 0; i < players.Count; i++)
            {
                var matchPlayer = match.Players[i];
                players[i].Ini(i, matchPlayer);
            }
        }
    }
}