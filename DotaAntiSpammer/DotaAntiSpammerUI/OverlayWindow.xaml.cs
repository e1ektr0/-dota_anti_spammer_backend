using System;
using System.Windows.Input;
using System.Windows.Media;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet.Common;

namespace DotaAntiSpammerNet
{
    public partial class OverlayWindow
    {

        public OverlayWindow()
        {
            InitializeComponent();
        }

        public event EventHandler<DrawingContext> Draw;

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

        protected override void OnRender(DrawingContext drawingContext)
        {
            OnDraw(drawingContext);
            base.OnRender(drawingContext);
        }

        protected virtual void OnDraw(DrawingContext e)
        {
            Draw?.Invoke(this, e);
        }

        private void OverlayWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("sdasdasdas");
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