using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace DotaAntiSpammerNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var interop = new WindowInteropHelper(this);
            interop.EnsureHandle();
            this.Topmost = true;
   
            var parentWindowHandler = IntPtr.Zero;
            // I'll just look for notepad window so I can demonstrate (remember to run notepad before running this sample code :))
            foreach (var pList in Process.GetProcesses())
            {
                if (pList.ProcessName.Contains("dota2"))
                {
                    parentWindowHandler = pList.MainWindowHandle;
                    break;
                }
            }
            // this is it
            interop.Owner = parentWindowHandler;
            
            // i'll use this to check if owner is set
            // if it's set MainWindow will be shown at the center of notepad window
            WindowStartupLocation=WindowStartupLocation.Manual;
            Left = 12;
            Top = 55;
            // window.Left  = location.X;
            // window.Top   = location.Y - window.Height;
            MouseDown += Window_MouseDown;
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // if (e.ChangedButton == MouseButton.Left)
            //     DragMove();
            // Console.WriteLine($"{Left}-{Top}");
        }
    }
}