using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Win32;

namespace DotaAntiSpammerLauncher
{
    public static class FileManagement
    {
        private static string _serverLog;

        public static string ServerLogPath
        {
            get
            {
                if (_serverLog != null)
                    return _serverLog;

                foreach (var path in SteamAppDirectories)
                {
                    if (!File.Exists(path + "\\common\\dota 2 beta\\game\\dota\\server_log.txt"))
                        continue;
                    _serverLog = path + "\\common\\dota 2 beta\\game\\dota\\server_log.txt";
                    break;
                }

                return _serverLog;
            }
        }

        public static List<string> GetPlayerIDs()
        {
            var gameInfo = GetLastLobby(ServerLogPath);

            var playerStartIndex = gameInfo.IndexOf('(') + 1;
            var playerEndIndex = gameInfo.IndexOf(')');
            var playerSection = gameInfo.Substring(playerStartIndex, playerEndIndex - playerStartIndex);

            var players = playerSection.Split(' ').Where(x => x.Contains("[U:")).Take(10).ToList();

            var results = new List<string>();

            foreach (var item in players)
            {
                var startIndex = item.LastIndexOf(':') + 1;
                var endIndex = item.IndexOf(']');
                var length = endIndex - startIndex;

                results.Add(item.Substring(startIndex, length));
            }

            return results;
        }


        private static string _steamInstallPath;

        private static string SteamInstallPath =>
            _steamInstallPath ?? (_steamInstallPath =
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "") as string);

        private static List<string> SteamAppDirectories
        {
            get
            {
                var steamAppDirectories = new List<string>() {SteamInstallPath + "\\steamapps"};

                var lines = File.ReadAllLines(SteamInstallPath + "\\steamapps\\libraryfolders.vdf");

                for (var i = 4; i < lines.Length - 1; i++)
                {
                    var index = lines[i].IndexOfNth("\"", 3);
                    steamAppDirectories.Add(lines[i]
                        .Substring(index + 1, lines[i].Length - (index + 2)) + "\\steamapps");
                }

                return steamAppDirectories;
            }
        }

        private static int IndexOfNth(this string str, string value, int nth = 1)
        {
            if (nth <= 0)
                throw new ArgumentException("Can not find the zeroth index of substring in string. Must start with 1");
            var offset = str.IndexOf(value, StringComparison.Ordinal);
            for (var i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value, offset + 1, StringComparison.Ordinal);
            }

            return offset;
        }

        public static string GetLastLobby(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            try
            {
                var lastOrDefault = File.ReadAllLines(filePath).LastOrDefault(x => x.Contains("Lobby"));
                return lastOrDefault;
            }
            catch (Exception e)
            {
                Thread.Sleep(1000);
                return File.ReadAllLines(filePath).LastOrDefault(x => x.Contains("Lobby"));
            }
        }
    }
}