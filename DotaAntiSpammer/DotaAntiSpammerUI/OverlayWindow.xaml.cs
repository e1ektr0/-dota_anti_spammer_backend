using System;
using System.Windows;
using System.Windows.Media;
using DotaAntiSpammerCommon;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet.Common;
using Process.NET.Windows;

namespace DotaAntiSpammerNet
{
    public partial class OverlayWindow : Window
    {
        private readonly IWindow _targetWindow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OverlayWindow" /> class.
        /// </summary>
        /// <param name="targetWindow">The window.</param>
        /// <param name="match"></param>
        public OverlayWindow(IWindow targetWindow, Match match)
        {
            _targetWindow = targetWindow;
            InitializeComponent();
            Match.Ini(match);
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
    }
}