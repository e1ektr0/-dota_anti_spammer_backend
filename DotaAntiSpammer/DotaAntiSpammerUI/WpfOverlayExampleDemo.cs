using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Windows;
using DotaAntiSpammerCommon;
using DotaAntiSpammerNet.Common;
using Process.NET;
using Process.NET.Memory;

namespace DotaAntiSpammerNet
{
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo
    {
        /// <summary>
        ///     The overlay
        /// </summary>
        public static OverlayPlugin Overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private static ProcessSharp _processSharp;


        public OverlayWindow OverlayWindow;

        /// <summary>
        ///     Starts the demo.
        /// </summary>
        /// <param name="getPlayerIDs"></param>
        public void StartDemo(List<string> getPlayerIDs)
        {
            if (getPlayerIDs == null)
                throw new ArgumentNullException(nameof(getPlayerIDs));

            // Set up objects/overlay
            var process = System.Diagnostics.Process.GetProcessesByName("dota2").FirstOrDefault();
            if (process == null)
                return;

            try
            {
                var description = new WebClient().DownloadString(GlobalConfig.ApiUrl + GlobalConfig.StatsUrl +
                                                                 "?accounts=" + string.Join(",", getPlayerIDs));
                var match = JsonSerializer.Deserialize<DotaAntiSpammerCommon.Models.Match>(description,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                match.Sort(getPlayerIDs);
                _processSharp = new ProcessSharp(process, MemoryType.Remote);
                Overlay = new WpfOverlayDemoExample();
                var wpfOverlay = (WpfOverlayDemoExample) Overlay;
                OverlayWindow = new OverlayWindow();
                OverlayWindow.Ini(match);
                wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow, _processSharp.Handle, OverlayWindow);
                wpfOverlay.Enable();

                OverlayWindow.Visibility = Visibility.Hidden;
                new Application().Run(OverlayWindow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

      
    }
}