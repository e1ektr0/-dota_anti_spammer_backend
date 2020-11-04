using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using DotaAntiSpammerCommon;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerNet;
using Microsoft.Win32;
using Application = System.Windows.Forms.Application;

namespace DotaAntiSpammerLauncher
{
    internal static class Program
    {
        private static LowLevelKeyboardHook _kbh;
        private static OverlayWindow _window;
        private static string _lastLobby;
        private static bool _altPressed;
        private static FileSystemWatcher _watcher;

        [STAThread]
        public static void Main()
        {
            try
            {
                _lastLobby = FileManagement.GetLastLobby(FileManagement.ServerLogPath);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                _window = new OverlayWindow {Visibility = Visibility.Visible};
                AddHooks();

                AddFileWatcher();

                var application = new System.Windows.Application();
                LoadData(_window);

                application.Run(_window);
            }
            catch (Exception e)
            {
                File.WriteAllText("log", e.Message);
                throw;
            }
        }


        private static void AddFileWatcher()
        {
            var serverLogPath = FileManagement.ServerLogPath;
            while (serverLogPath == null)
            {
                Console.WriteLine(@"Server log not founded");
                Thread.Sleep(2000);
                serverLogPath = FileManagement.ServerLogPath;
            }

            var fileInfo = new FileInfo(serverLogPath);
            var fileInfoDirectory = fileInfo.Directory;
            if (fileInfoDirectory == null)
            {
                Console.WriteLine(@"Directory not found");
                return;
            }

            _watcher = new FileSystemWatcher(fileInfoDirectory.FullName)
            {
                EnableRaisingEvents = true
            };

            _watcher.Changed += (o, a) =>
            {
                try
                {
                    var tempLobby = FileManagement.GetLastLobby(serverLogPath);
                    if (_lastLobby == tempLobby)
                        return;

                    _watcher.EnableRaisingEvents = false;
                    LoadData(_window);
                    _window.ShowInvoke();
                    _lastLobby = tempLobby;
                }
                finally
                {
                    _watcher.EnableRaisingEvents = true;
                }
            };
        }

        private static void AddHooks()
        {
            _kbh = new LowLevelKeyboardHook();
            _kbh.OnKeyPressed += (sender, keys) =>
            {
                if (keys == Keys.LMenu)
                {
                    _altPressed = true;
                }

                if (keys != Keys.Oemtilde || !_altPressed)
                    return;

                LoadData(_window);
                _window.ShowHideInvoke();
            };
            _kbh.OnKeyUnpressed += (sender, keys) =>
            {
                if (keys != Keys.LMenu)
                    return;
                _altPressed = false;
            };
            _kbh.HookKeyboard();
        }

        private static void LoadData(OverlayWindow window)
        {
            window.Dispatcher.Invoke(() =>
            {
                window.Ini(new Match
                {
                    Players = new List<Player> {null, null, null, null, null, null, null, null, null, null}
                });
            });
            var match = new Match
            {
                Players = new List<Player>()
            };
            var playerIDs = FileManagement.GetPlayerIDs();
            try
            {
                var statsUrl = GlobalConfig.ApiUrl + GlobalConfig.StatsUrl;
                var currentId = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam\ActiveProcess", "ActiveUser",
                    (int)0);
                var url = statsUrl + "?accounts=" + string.Join(",", playerIDs) + "&currentId=" + currentId;
                match.CurrentId = (int)currentId; 
                var description = new WebClient().DownloadString(url);
                match = JsonSerializer.Deserialize<Match>(description,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                match.Sort(playerIDs);
                match.CurrentId = (int)currentId; 
                window.Dispatcher.Invoke(() => { window.Ini(match); });
            }
            catch (Exception e)
            {
                match.Sort(playerIDs);
                Console.WriteLine(e);
            }

        }
    }
}