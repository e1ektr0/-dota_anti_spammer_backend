using System;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet.Common;
using Process.NET.Native.Types;
using Process.NET.Windows;

namespace DotaAntiSpammerNet
{
    public class WpfOverlayDemoExample : WpfOverlayPlugin
    {
        private readonly Match _match;

        // Used to limit update rates via timestamps 
        // This way we can avoid thread issues with wanting to delay updates
        private readonly TickEngine _tickEngine = new TickEngine();

        private bool _isDisposed;

        private bool _isSetup;

        private int _temp;

        public WpfOverlayDemoExample(Match match)
        {
            _match = match;
        }

        // Shapes used in the demo

        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        protected override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Initialize(IWindow targetWindow, SafeMemoryHandle processSharpHandle,
            OverlayWindow overlayWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow, processSharpHandle, overlayWindow);

            OverlayWindow = overlayWindow;
            
            _tickEngine.Interval = TimeSpan.FromMilliseconds(16);
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            if (OverlayWindow.IsVisible) OverlayWindow.Update();
        }

        private void OnPreTick(object sender, EventArgs eventArgs)
        {
            // Only want to set them up once.
            if (!_isSetup)
            {
                SetUp();
                _isSetup = true;
            }

            var activated = TargetWindow.IsActivated;
            var visible = OverlayWindow.IsVisible;

            // Ensure window is shown or hidden correctly prior to updating
            if (!activated && visible)
            {
                _temp++;
                if (_temp > 10)
                    OverlayWindow.Hide();
            }
            else
            {
                _temp = 0;
            }

            if (!activated || visible)
                return;

            OverlayWindow.Show();
        }

        public override void Update()
        {
            _tickEngine.Pulse();
        }

        // Clear objects
        public override void Dispose()
        {
            if (_isDisposed) return;

            if (IsEnabled) Disable();

            OverlayWindow?.Hide();
            OverlayWindow?.Close();
            OverlayWindow = null;
            _tickEngine.Stop();

            base.Dispose();
            _isDisposed = true;
        }

        ~WpfOverlayDemoExample()
        {
            Dispose();
        }

        private void SetUp()
        {
        }
    }
}