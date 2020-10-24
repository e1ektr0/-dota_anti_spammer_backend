using System.Collections.Generic;
using System.Windows.Media;

namespace DotaAntiSpammerNet.models
{
    public static class PlayerColors
    {
        public static readonly Dictionary<int, Color> Colors = new Dictionary<int, Color>
        {
            {
                0, Color.FromRgb(52, 118, 255)
            },
            {
                1, Color.FromRgb(75, 191, 143)
            },
            {
                2, Color.FromRgb(139, 0, 139)
            },
            {
                3, Color.FromRgb(182, 180, 6)
            },
            {
                4, Color.FromRgb(189, 78, 0)
            },
            {
                5, Color.FromRgb(254, 135, 195)
            },
            {
                6, Color.FromRgb(162, 181, 72)
            },
            {
                7, Color.FromRgb(102, 217, 247)
            },
            {
                8, Color.FromRgb(0, 132, 34)
            },
            {
                9, Color.FromRgb(165, 106, 0)
            }
        };
    }
}