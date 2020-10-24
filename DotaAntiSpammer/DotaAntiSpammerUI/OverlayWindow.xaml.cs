using System;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet.Common;

namespace DotaAntiSpammerNet
{
    public sealed partial class OverlayWindow
    {

        public OverlayWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        public void Update()
        {
            Left = 12;
            Top = 55;
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.MakeWindowTransparent();
        }

        public void Ini(Match match)
        {
            Dispatcher.Invoke(() =>
            {
                Match.Ini(match);
            });
        }
    }
}