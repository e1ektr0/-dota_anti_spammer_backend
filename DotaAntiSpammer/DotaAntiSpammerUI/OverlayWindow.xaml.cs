using System;
using System.Windows;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet.Common;

namespace DotaAntiSpammerNet
{
    public sealed partial class OverlayWindow
    {
        private bool _notShowedYet;

        public OverlayWindow()
        {
            InitializeComponent();
            _notShowedYet = true;
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
            Match.Ini(match);
        }

        public void ShowHideInvoke()
        {
            Dispatcher.Invoke(() =>
            {
                if (_notShowedYet)
                {
                    Show();
                    _notShowedYet = false;
                    return;
                }
                if(Visibility == Visibility.Hidden || Visibility == Visibility.Collapsed)
                    Show();
                else
                    Hide();
            });
        }

        public void ShowInvoke()
        {
            Dispatcher.Invoke(Show);
            _notShowedYet = false;
        }
    }
}