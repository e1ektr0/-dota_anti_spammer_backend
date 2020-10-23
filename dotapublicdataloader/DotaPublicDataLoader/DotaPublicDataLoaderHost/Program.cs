﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace DotaPublicDataLoaderHost
{
    enum Clusters
    {
        Usa2 = 110,
        Usa = 120,
        Europx = 130, //
        Korea = 140, //140+
        AsiaSea = 150, //151+
        Dubai = 160,
        Australia = 170,
        Rus = 180,
        Europ2 = 180,
        Europ = 190,
        SourceAmerica = 200,
        Affrica = 210,
        China = 220,
        Chili = 240,
        Peru = 250,
        India = 260
    }

    internal static class Program
    {
        private static string _key = "D356E276EC76CE809A433AC8A0B81952";

        private static readonly List<Clusters> SupportedClusters = new List<Clusters>
        {
            DotaPublicDataLoaderHost.Clusters.Europx, DotaPublicDataLoaderHost.Clusters.Europ2,
            DotaPublicDataLoaderHost.Clusters.Europ,
            DotaPublicDataLoaderHost.Clusters.Rus
        };

        private static readonly Dictionary<int, UInt64> Clusters = new Dictionary<int, UInt64>();

        //5662200710
        private static void Main()
        {
            
            var matchDetailses = GetMatchx(5666916014);
            var detailses = GetMatches().FirstOrDefault(n=>n.match_id == 5666916014);
            var preparedClustersId = SupportedClusters.Select(n => (int) n / 10);
            var repository = new MongoRepository();
            var seq = repository.GetLastSeq();
            var count = 0;
            var startTime = DateTime.UtcNow;
            seq ??= GetSeq();
            while (true)
            {
                var matches = GetMatchesStartingOnSeqNumber(seq.Value);
                var newCount = matches.Count;

                if (matches.Any())
                {
                    seq = matches.Max(n => n.match_seq_num);
                }

                matches = matches.Where(n => preparedClustersId.Contains(n.cluster / 10)).ToList();
                matches = matches.Where(n => n.game_mode == 22).ToList(); //ranked ap
                matches = matches.Where(n => n.lobby_type == 7).ToList(); //ranked ap

                count += matches.Count();
                Thread.Sleep(newCount == 100 ? 1000 : 5000);

                if (matches.Any())
                    repository.Insert(matches);
                repository.UpdateMaxSeq(seq);
                Console.WriteLine(
                    $"Total loaded:{count} after {DateTime.UtcNow - startTime}. New matches:{newCount}. Add to queue: {matches.Count}");
              
            }
        }

        private static ulong GetSeq()
        {
            var matches = GetMatches();
            var max = matches.Any() ? matches.Max(n => n.match_seq_num) : 4763202335;
            return Math.Max(max, 4763202335);
        }

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            try
            {
                var url =
                    $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={_key}";
                var json = new WebClient().DownloadString(url);
                var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
                return matchHistory.result.matches ?? new List<MatchDetails>();
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                Thread.Sleep(30 * 1000);
                return GetMatchesStartingOnSeqNumber(seqNumber);
            }
        }

        private static IList<MatchDetails> GetMatches()
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001?key={_key}&start_at_match_id=5666916013";
            var json = new WebClient().DownloadString(url);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
        }

        private static IList<MatchDetails> GetMatch(ulong matchId)
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/V001/?match_id={matchId}&key={_key}";
            var json = new WebClient().DownloadString(url);
            //    var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            //  return matchHistory.result.matches;
            return null;
        }
        private static IList<MatchDetails> GetMatchx(ulong matchId)
        {
            var url =
                $"https://api.steampowered.com/IEconDOTA2_570/GetHeroes/v0001/?language=en_en&key={_key}";
            var json = new WebClient().DownloadString(url);
            //    var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            //  return matchHistory.result.matches;
            return null;
        }
    }
}