using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DotaAntiSpammerNet.models;

namespace DotaAntiSpammerNet.Controls.Bans
{
    public partial class Bans : UserControl
    {
        public Bans()
        {
            InitializeComponent();
        }

        public void Ini(List<int> toList)
        {
            var images = new List<Image> {Hero1, Hero2, Hero3 };
            foreach (var image in images)
            {
                image.Visibility = Visibility.Collapsed;
            }

            if (toList == null)
                return;
            
            for (var i = 0; i < toList.Count; i++)
            {
                var instanceHero = HeroConfigAll.Instance.Heroes.FirstOrDefault(n=>n.Id ==toList[i]);
                if (instanceHero == null)
                {
                    images[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    var bitmapImage = new BitmapImage(new Uri($"../../icons/{instanceHero.Name}.png", UriKind.Relative));
                    images[i].Source = bitmapImage;
                    images[i].Visibility = Visibility.Visible;

                }
            }
        }
    }
}