using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Hero
{
    public partial class HeroSmall : UserControl
    {
        public HeroSmall()
        {
            InitializeComponent();
        }

        public void Ini(DotaAntiSpammerCommon.Models.Hero hero)
        {
            var instanceHero = HeroConfigAll.Instance.Heroes[hero.Id];

            Image.Source = new BitmapImage(new Uri($"../../icons/{instanceHero.Name}.png", UriKind.Relative));
            Games.Text = hero.Games.ToString();
            WinRate.Text = ((int) hero.WinRate).ToString(CultureInfo.InvariantCulture);
            WinRate.Foreground = hero.WinRate > 50 ? Brushes.Green : Brushes.Red;
        }
    }
}