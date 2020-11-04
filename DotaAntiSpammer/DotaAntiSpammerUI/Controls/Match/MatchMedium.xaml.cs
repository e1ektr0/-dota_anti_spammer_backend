using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
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
            if (match?.Players == null)
                return;

            var player = match.Players.FirstOrDefault(n => n?.AccountId == match.CurrentId);
            if (player != null)
            {
                var index = match.Players.IndexOf(player);
                var border = BorderA;
                var enemy = match.Players.Take(5).ToList();
                if (index < 5)
                {
                    border = BorderB;
                    enemy = match.Players.Skip(5).ToList();
                }

                var orderByDescending = enemy.Where(n => n != null)
                    .SelectMany(n => n.Heroes.Select(x => new {points = x.WinRate * x.Games, heroId = x.Id}))
                    .OrderByDescending(n => n.points).ToList();
                BansX.Ini(orderByDescending.Select(n=>n.heroId).Take(3).ToList());
                border.BorderBrush = Brushes.Red;
            }
            else
            {
                BansX.Ini(null);
            }

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