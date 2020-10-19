using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace DotaPublicDataLoaderHost
{
    enum Clusters
    {
        Usa2= 110,
        Usa= 120,
        Europx = 130,//
        Korea = 140,//140+
        AsiaSea = 150,//151+
        Dubai = 160,
        Australia = 170,
        Rus = 180,
        Europ2 = 180,
        Europ = 190,
        SourceAmerica = 200,
        Affrica = 210,
        China = 220,
        Chili = 240,
        Peru=250,
        India = 260
    }
    
    internal static class Program
    {
        private static string _key = "D356E276EC76CE809A433AC8A0B81952";

        private static readonly List<Clusters> SupportedClusters = new List<Clusters>
        {
            DotaPublicDataLoaderHost.Clusters.Europx, DotaPublicDataLoaderHost.Clusters.Europ2, DotaPublicDataLoaderHost.Clusters.Europ,
            DotaPublicDataLoaderHost.Clusters.Rus
        };

        private static readonly Dictionary<int, UInt64> Clusters = new Dictionary<int, UInt64>();
        //5662200710
        private static void Main()
        {
            //var b = (int)SupportedClusters[0] / 10 == 131 / 10;
            Console.WriteLine(uint.MaxValue);
            var preparedClustersId = SupportedClusters.Select(n => (int) n / 10);
            var matchesRepository = new MatchesRepository();
            var lastSeq = matchesRepository.GetLastSeq();
            int count = 0;
            while (true)
            {
                lastSeq ??= GetSeq();
                var matches = GetMatchesStartingOnSeqNumber(lastSeq.Value + 1);
                matches = matches.Where(n => preparedClustersId.Contains(n.cluster / 10)).ToList();
                foreach (var match in matches)
                {
                    match.private_data_loaded = match.players.All(n => n.account_id != uint.MaxValue);
                }

                count += matches.Count(n=>!n.private_data_loaded);
                Thread.Sleep(1000);
                if (matches.Any())
                    matchesRepository.Insert(matches);
                if(matches.Any())
                    lastSeq = matches.Max(n => n.match_seq_num);
                Thread.Sleep(1);
                Console.WriteLine($"{count}");
            }
        }

        private static ulong GetSeq()
        {
            var matches = GetMatches();
            return matches.Any() ? matches.Max(n=>n.match_seq_num) : (ulong) 4756161920;
        }

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            try
            {
                var url =
                    $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={_key}";
                var json = new WebClient().DownloadString(url);
                var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
                return matchHistory.result.matches;
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                Thread.Sleep(30*1000);
                return GetMatchesStartingOnSeqNumber(seqNumber);
            }
        }
        
        private static IList<MatchDetails> GetMatches()
        {
            var url = $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001?key={_key}";
            var json = new WebClient().DownloadString(url);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
        }
    }
}