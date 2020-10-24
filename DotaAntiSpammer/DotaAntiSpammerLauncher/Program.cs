using System;
using Overlay.NET.Demo.Wpf;

namespace DotaAntiSpammerLauncher
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var wpfDemo = new WpfOverlayExampleDemo();
            wpfDemo.StartDemo();
            Console.ReadLine();
        }
    }
}