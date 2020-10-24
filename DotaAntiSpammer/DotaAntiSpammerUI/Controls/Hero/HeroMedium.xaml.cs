using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Hero
{
    public partial class HeroMedium
    {
        public HeroMedium()
        {
            InitializeComponent();
        }

        public void Ini(DotaAntiSpammerCommon.Models.Hero hero)
        {
            Visibility = Visibility.Visible;
            var instanceHero = HeroConfigAll.Instance.Heroes.FirstOrDefault(n=>n.Id == hero.Id);
            if (instanceHero == null)
            {
                Visibility = Visibility.Hidden;
                return;
            }
            Image.Source = new BitmapImage(new Uri($"../../icons/{instanceHero.Name}.png", UriKind.Relative));
            Games.Text = hero.Games.ToString();
            WinRate.Text = $"{(int) hero.WinRate}%";
            WinRate.Foreground = hero.WinRate > 50 ? Brushes.Green : Brushes.Red;
            SpamIcon.Visibility = hero.Spam ? Visibility.Visible : Visibility.Collapsed;
            if (hero.PickStage != null)
            {
                PickStageContainer.Visibility = Visibility.Visible;
                PickStage.Text = hero.PickStage.ToString();
            }
            else
            {
                PickStageContainer.Visibility = Visibility.Collapsed;
            }
        }
    }
}