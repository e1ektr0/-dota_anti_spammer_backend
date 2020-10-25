using System;
using System.Linq;
using System.Windows;
using DotaAntiSpammerNet.Common;
using Process.NET;
using Process.NET.Memory;

namespace DotaAntiSpammerNet
{
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo
    {

        public OverlayWindow OverlayWindow;

        /// <summary>
        ///     Starts the demo.
        /// </summary>
        /// <param name="getPlayerIDs"></param>
        public void StartOverlay()
        {
            try
            {
                
                OverlayWindow = new OverlayWindow {Visibility = Visibility.Hidden};
                var application = new Application();
                application.Run(OverlayWindow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

      
    }
}