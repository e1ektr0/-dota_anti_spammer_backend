using System.Linq;
using Overlay.NET.Common;
using Process.NET;
using Process.NET.Memory;

namespace Overlay.NET.Demo.Wpf {
    /// <summary>
    /// </summary>
    public class WpfOverlayExampleDemo {
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
        public void StartDemo() {

            // Set up objects/overlay
            System.Diagnostics.Process process;
            while (true)
            {
                var processName = "dota2";
                process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
                if (process != null) {
                   break;
                }
            }
            
            _processSharp = new ProcessSharp(process, MemoryType.Remote);
            _overlay = new WpfOverlayDemoExample();

            var wpfOverlay = (WpfOverlayDemoExample) _overlay;

            wpfOverlay.Initialize(_processSharp.WindowFactory.MainWindow, _processSharp.Handle);
            wpfOverlay.Enable();

            _work = true;

            // Do work
            while (_work) {
                _overlay.Update();
            }

            Log.Debug("Demo complete.");
        }
    }
}