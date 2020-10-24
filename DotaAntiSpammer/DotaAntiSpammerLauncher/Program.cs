using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using DotaAntiSpammerCommon;
using DotaAntiSpammerNet;
using GEst;

namespace DotaAntiSpammerLauncher
{
    internal static class Program
    {
        private static string LastLobby { get; set; }
        private static readonly AutoResetEvent  Handler = new AutoResetEvent(false);
        private static int _pressed;

        [STAThread]
        public static void Main(string[] args)
        {
            var iDs = FileManagement.GetPlayerIDs();
            LastLobby = FileManagement.GetLastLobby(FileManagement.ServerLogPath);
            var fileInfo = new FileInfo(FileManagement.ServerLogPath);
            var wpfDemo1 = new WpfOverlayExampleDemo();

            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += (sender, keys) =>
            {
                if (keys != Keys.F1) 
                    return;
                
                wpfDemo1.OverlayWindow.ShowHide();
            };
            kbh.HookKeyboard();
          
            var fileInfoDirectory = fileInfo.Directory;
            if (fileInfoDirectory == null)
            {
                Console.WriteLine(@"Directory not found");
                return;
            }

            var watcher = new FileSystemWatcher(fileInfoDirectory.FullName)
            {
                EnableRaisingEvents = true
            };

            watcher.Changed += (o, a) =>
            {
                try
                {
                    var tempLobby = FileManagement.GetLastLobby(FileManagement.ServerLogPath);
                    if (LastLobby == tempLobby)
                        return;

                    watcher.EnableRaisingEvents = false;
                    var playerIDs = FileManagement.GetPlayerIDs();
                    var description = new WebClient().DownloadString(GlobalConfig.ApiUrl + GlobalConfig.StatsUrl +
                                                                     "?accounts=" + string.Join(",", playerIDs));
                    var match = JsonSerializer.Deserialize<DotaAntiSpammerCommon.Models.Match>(description,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    match.Sort(playerIDs);
                    wpfDemo1.OverlayWindow.Dispatcher.Invoke(() =>
                    {
                        wpfDemo1.OverlayWindow.Ini(match);
                    });
                    wpfDemo1.OverlayWindow.ShowI();
                    LastLobby = tempLobby;
                }
                finally
                {
                    watcher.EnableRaisingEvents = true;
                }
            };
            wpfDemo1.StartDemo( FileManagement.GetPlayerIDs());  
        }
    }
}