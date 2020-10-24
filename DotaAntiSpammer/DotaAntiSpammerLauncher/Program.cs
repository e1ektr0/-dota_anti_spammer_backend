using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotaAntiSpammerNet;

namespace DotaAntiSpammerLauncher
{
    internal static class Program
    {
        private static string LastLobby { get; set; }
        private static readonly AutoResetEvent  Handler = new AutoResetEvent(false);
        [STAThread]
        public static void Main(string[] args)
        {
            var iDs = FileManagement.GetPlayerIDs();
            LastLobby = FileManagement.GetLastLobby(FileManagement.ServerLogPath);
            var fileInfo = new FileInfo(FileManagement.ServerLogPath);
            var wpfDemo1 = new WpfOverlayExampleDemo();

          
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

                    Handler.Set();
                    LastLobby = tempLobby;
                }
                finally
                {
                    watcher.EnableRaisingEvents = true;
                }
            };

            while (true)
            {
                Handler.WaitOne();
                var playerIDs = FileManagement.GetPlayerIDs();
                wpfDemo1.StartDemo(playerIDs);
            }          
        }
    }
}