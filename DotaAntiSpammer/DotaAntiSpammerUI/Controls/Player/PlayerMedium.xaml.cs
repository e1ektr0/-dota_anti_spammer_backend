﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using DotaAntiSpammerNet.Controls.Hero;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Player
{
    public partial class PlayerMedium : UserControl
    {
        private readonly List<HeroMedium> _heroes;

        public PlayerMedium()
        {
            InitializeComponent();
            _heroes = new List<HeroMedium>
            {
                Hero0,
                Hero1,
                Hero2
            };
        }

        public void Ini(int i, DotaAntiSpammerCommon.Models.Player player)
        {
            Border.BorderBrush = new SolidColorBrush(PlayerColors.Colors[i]);
            for (var j = 0; j < player.Heroes.Count && j < _heroes.Count; j++)
                _heroes[j].Ini(player.Heroes[j]);
            
            

            Games.Text = $"{player.TotalGames:0.00}";
            WinRate.Text = $"{player.WinRate:0.00}%";
        }
    }
}