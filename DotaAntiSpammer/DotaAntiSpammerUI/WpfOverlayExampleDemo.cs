using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using DotaAntiSpammerCommon;
using DotaAntiSpammerNet.Common;
using Process.NET;
using Process.NET.Memory;
using Match = System.Text.RegularExpressions.Match;

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
        public void StartDemo()
        {
            // Set up objects/overlay
            System.Diagnostics.Process process;
            while (true)
            {
                var processName = "dota2";
                process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null) break;
            }

            try
            {
                var description = new WebClient().DownloadString(GlobalConfig.ApiUrl+GlobalConfig.StatsUrl+"?accounts=1049054702,134556694,132851371,890776708,252473997,875081338,204132249,488978443,898374197,157924451");
                var match = JsonSerializer.Deserialize< DotaAntiSpammerCommon.Models.Match>(description, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _processSharp = new ProcessSharp(process, MemoryType.Remote);
                _overlay = new WpfOverlayDemoExample(match);

                var wpfOverlay = (WpfOverlayDemoExample) _overlay;

                wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow, _processSharp.Handle);
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