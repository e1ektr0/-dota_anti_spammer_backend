using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using DotaAntiSpammerNet.Controls.Hero;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Player
{
    public partial class PlayerSmall : UserControl
    {
        private readonly List<HeroSmall> _heroes;

        public PlayerSmall()
        {
            InitializeComponent();
            _heroes = new List<HeroSmall>
            {
                Hero0,
                Hero1,
                Hero2,
                Hero3,
                Hero4,
                Hero5,
                Hero6,
                Hero7,
                Hero8
            };
        }

        public void Ini(int i, DotaAntiSpammerCommon.Models.Player player)
        {
            Border.BorderBrush = new SolidColorBrush(PlayerColors.Colors[i]);
            for (var j = 0; j < player.Heroes.Count && j < _heroes.Count; j++) _heroes[j].Ini(player.Heroes[j]);

            Games.Text = $"{player.TotalGames}";
            WinRate.Text = $"{player.WinRate}%";
        }
    }
}