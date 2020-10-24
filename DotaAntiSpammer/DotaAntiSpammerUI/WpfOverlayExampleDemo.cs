using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
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
        private static OverlayPlugin _overlay;

        /// <summary>
        ///     The process sharp
        /// </summary>
        private static ProcessSharp _processSharp;

        /// <summary>
        ///     The work
        /// </summary>
        private static bool _work;

        /// <summary>
        ///     Starts the demo.
        /// </summary>
        /// <param name="getPlayerIDs"></param>
        /// <param name="overlayWindow"></param>
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
                _processSharp = new ProcessSharp(process, MemoryType.Remote);
                _overlay = new WpfOverlayDemoExample(match);

                var wpfOverlay = (WpfOverlayDemoExample) _overlay;
                var overlayWindow = new OverlayWindow();
                overlayWindow.Ini(match);
                wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow, _processSharp.Handle, overlayWindow);
                wpfOverlay.Enable();

                _work = true;

                // Do work
                while (_work) _overlay.Update();

                Log.Debug("Demo complete.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}