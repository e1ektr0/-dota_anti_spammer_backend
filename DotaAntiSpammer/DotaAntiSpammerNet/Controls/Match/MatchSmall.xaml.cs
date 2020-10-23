﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet
{
    public partial class MatchSmall
    {
        public MatchSmall()
        {
            var sample = Match.Sample();
            InitializeComponent();
            var players = new List<PlayerSmall>
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
                players[i].Ini(i,sample.Players[i]);
            }
        }
    }
}