using System.Collections.Generic;
using System.Windows.Controls;

namespace DotaAntiSpammer
{
    /// <summary>
    /// Логика взаимодействия для Team.xaml
    /// </summary>
    public partial class Team : UserControl
    {
        private readonly List<IPlayerComponent> Players;
        public Team()
        {
            InitializeComponent();
            Players = new List<IPlayerComponent> { Player1, Player2, Player3, Player4, Player5 };
            for (var i = 0; i < Players.Count; i++)
            {
                Players[i].Ini(i);
            }
        }
    }
}
