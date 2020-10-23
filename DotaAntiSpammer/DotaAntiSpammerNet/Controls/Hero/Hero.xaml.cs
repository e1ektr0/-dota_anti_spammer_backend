using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DotaAntiSpammerNet
{
    public partial class Hero : UserControl
    {
        public Hero()
        {
            InitializeComponent();
            Ini();
        }

        public void Ini()
        {
            Image.Source = new BitmapImage(new Uri("../../small/abaddon.png", UriKind.Relative));
        }
    }
}