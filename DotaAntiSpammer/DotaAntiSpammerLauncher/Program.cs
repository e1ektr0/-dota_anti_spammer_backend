using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using System.Windows.Forms;
using DotaAntiSpammerCommon;
using DotaAntiSpammerNet;
using Application = System.Windows.Forms.Application;

namespace DotaAntiSpammerLauncher
{
    internal static class Program
    {
        private static string LastLobby { get; set; }

        [STAThread]
        public static void Main(string[] args)
        {
            var window = new OverlayWindow {Visibility = Visibility.Hidden};
            LastLobby = FileManagement.GetLastLobby(FileManagement.ServerLogPath);
            var fileInfo = new FileInfo(FileManagement.ServerLogPath);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += (sender, keys) =>
            {
                if (keys != Keys.F1)
                    return;

                window.ShowHideInvoke();
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
                    LoadData(window);
                    window.ShowInvoke();
                    LastLobby = tempLobby;
                }
                finally
                {
                    watcher.EnableRaisingEvents = true;
                }
            };

            var application = new System.Windows.Application();
            application.Run(window);
            LoadData(window);
        }

        private static void LoadData(OverlayWindow window)
        {
            var playerIDs = FileManagement.GetPlayerIDs();
            var description = new WebClient().DownloadString(GlobalConfig.ApiUrl + GlobalConfig.StatsUrl +
                                                             "?accounts=" + string.Join(",", playerIDs));
            var match = JsonSerializer.Deserialize<DotaAntiSpammerCommon.Models.Match>(description,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            match.Sort(playerIDs);
            window.Dispatcher.Invoke(() => { window.Ini(match); });
        }
    }
}