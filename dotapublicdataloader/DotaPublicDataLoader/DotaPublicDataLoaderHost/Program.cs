using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace DotaPublicDataLoaderHost
{
    internal static class Program
    {
        private static string _key = "D356E276EC76CE809A433AC8A0B81952";

        //5662200710
        private static void Main(string[] args)
        {
            var matchesRepository = new MatchesRepository();
            var lastSeq = matchesRepository.GetLastSeq();
            while (true)
            {
                Stopwatch st = new Stopwatch();
                var matches = GetMatchesStartingOnSeqNumber(lastSeq+1);
                if(matches.Any())
                    matchesRepository.Insert(matches);
                lastSeq = matches.Max(n => n.match_seq_num);
                Thread.Sleep(1);
                Console.WriteLine(lastSeq);
            }
        }

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            var request =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={_key}";
            var json = new WebClient().DownloadString(request);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
        }
    }
}