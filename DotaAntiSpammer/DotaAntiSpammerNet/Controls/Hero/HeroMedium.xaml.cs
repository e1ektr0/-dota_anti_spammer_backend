using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Hero
{
    public partial class HeroMedium : UserControl
    {
        public HeroMedium()
        {
            InitializeComponent();
        }
        
        public void Ini(models.Hero hero)
        {
            var instanceHero = HeroConfigAll.Instance.Heroes[hero.Id];

            Image.Source = new BitmapImage(new Uri($"../../icons/{instanceHero.Name}.png", UriKind.Relative));
            Games.Text = hero.Games.ToString();
            WinRate.Text = $"{(int) hero.WinRate}%";
            WinRate.Foreground = hero.WinRate > 50 ? Brushes.Green : Brushes.Red;
        }
    }
}