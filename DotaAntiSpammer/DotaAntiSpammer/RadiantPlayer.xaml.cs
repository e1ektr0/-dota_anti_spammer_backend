using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotaAntiSpammer
{
    /// <summary>
    /// Логика взаимодействия для RadiantPlayer.xaml
    /// </summary>
    public partial class RadiantPlayer : IPlayerComponent
    {
        public void Ini(int index)
        {
            ColorBox.Fill = new SolidColorBrush(PlayerColors.Colors[index]);
        }

        public RadiantPlayer()
        {
            InitializeComponent();
        }
    }
}
