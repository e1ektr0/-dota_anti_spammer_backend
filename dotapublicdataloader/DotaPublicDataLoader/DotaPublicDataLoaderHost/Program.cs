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
            var preparedClustersId = SupportedClusters.Select(n => (int) n / 10);
            var matchesRepository = new MatchesRepository();
            var seq = matchesRepository.GetLastSeq();
            var count = 0;
            var startTime = DateTime.UtcNow;
            while (true)
            {
                seq ??= GetSeq();
                var matches = GetMatchesStartingOnSeqNumber(seq.Value);
                var newCount = matches.Count;
                var filteredBySkill = 0;
                
                if (matches.Any())
                {
                    seq = matches.Max(n => n.match_seq_num);
                    // var matchesBySkill = GetMatches(matches.Max(n => n.match_id +1));
                    // var x = matches.Where(n => matchesBySkill.Any(u => u.match_id == n.match_id)).ToList();
                    // Console.WriteLine(seq);
                    // matches = matches.Where(n => matchesBySkill.Any(u => u.match_id == n.match_id)).ToList();
                    // filteredBySkill = newCount - matches.Count;
                    // Console.WriteLine(filteredBySkill);
                }

                matches = matches.Where(n => preparedClustersId.Contains(n.cluster / 10)).ToList();
                matches = matches.Where(n => n.game_mode == 22).ToList(); //ranked ap
                matches = matches.Where(n => n.lobby_type == 7).ToList(); //ranked ap
                // foreach (var match in matches)
                // {
                //     var matchDetailses = GetMatch(match.match_id);
                // }
                foreach (var match in matches)
                {
                    match.private_data_loaded = match.players.All(n => n.account_id != uint.MaxValue);
                }

                count += matches.Count(n => !n.private_data_loaded);
                Thread.Sleep(5000);
                if (matches.Any())
                    matchesRepository.Insert(matches);

                Console.WriteLine(
                    $"Total loaded:{count} after {DateTime.UtcNow - startTime}. New matches:{newCount}. Add to queue: {matches.Count} Filtered by skill: {filteredBySkill}");
            }
        }

        private static ulong GetSeq()
        {
            var matches = GetMatches();
            return matches.Any() ? matches.Max(n => n.match_seq_num) : (ulong) 4756161920;
        }

        private static ulong GetMatchId()
        {
            var matches = GetMatches();
            return matches.Any() ? matches.Max(n => n.match_id) : (ulong) 5664053717;
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

        private static IList<MatchDetails> GetMatches(ulong? startMatchId = null, byte? skill = null)
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001?key={_key}";
            if (startMatchId != null)
                url += "&start_at_match_id" + startMatchId;
            if (skill != null)
                url += "&skill=3";
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
    }
}