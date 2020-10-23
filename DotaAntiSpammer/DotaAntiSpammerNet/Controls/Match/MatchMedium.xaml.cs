using System.Collections.Generic;
using System.Windows.Controls;
using DotaAntiSpammerNet.Controls.Player;

namespace DotaAntiSpammerNet.Controls.Match
{
    public partial class MatchMedium : UserControl
    {
        public MatchMedium()
        {
            InitializeComponent();
            var sample = DotaAntiSpammerNet.models.Match.Sample();
            InitializeComponent();
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
                players[i].Ini(i, sample.Players[i]);
            }
        }
    }
}