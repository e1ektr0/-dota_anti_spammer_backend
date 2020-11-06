using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
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
            CreateTray();
        }

        private void CreateTray()
        {
            var trayIcon = new NotifyIcon {Text = @"Dota Anti Spammer"};
            var trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", (sender, args) =>
            {
                Close();
            });

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            
            trayIcon.Icon = new Icon("dota_anti_spam.ico");
            trayIcon.DoubleClick += (sender, args) =>
            {
                ShowHideInvoke();
            };
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
            Dispatcher?.Invoke(Show);
            _notShowedYet = false;
        }
        
    }
}