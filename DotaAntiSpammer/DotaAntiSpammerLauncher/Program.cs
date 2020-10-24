using System;
using DotaAntiSpammerNet;

namespace DotaAntiSpammerLauncher
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var wpfDemo = new WpfOverlayExampleDemo();
            wpfDemo.StartDemo();
        }
    }
}