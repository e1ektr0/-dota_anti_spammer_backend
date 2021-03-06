﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using DotaAntiSpammerMongo;
using DotaAntiSpammerMongo.Models.Match;

namespace DotaPublicDataLoaderHost
{
    internal enum Clusters
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
        private static readonly string _key = "D356E276EC76CE809A433AC8A0B81952";

        private static List<string> _keys = new List<string>
        {
            "D356E276EC76CE809A433AC8A0B81952",
            "88A096C96B6C9632E7897B677172890F",
        };

        private static readonly List<Clusters> SupportedClusters = new List<Clusters>
        {
            Clusters.Europx,
            Clusters.Europ2,
            Clusters.Europ,
            Clusters.Rus
        };

        private static string[] _proxyList;

        private static void Main()
        {
            _proxyList = File.ReadAllLines("proxy_http_ip.txt");
            var preparedClustersId = SupportedClusters.Select(n => (int) n / 10);
            var repository = new MongoRepository();
            var seq = repository.GetLastSeq();
            var count = 0;
            var startTime = DateTime.UtcNow;
            seq ??= GetSeq();
            while (true)
            {
                var matches = GetMatchesStartingOnSeqNumber(seq.Value + 1);
                if (matches == null)
                {
                    seq++;
                    continue;
                }

                if (!matches.Any())
                {
                    Thread.Sleep(5000);
                    continue;
                }

                var max = matches.Max(n => n.start_time + n.durtaion);
                var utcDateTime = DateTimeOffset.FromUnixTimeSeconds((long) max).UtcDateTime;
                if (DateTime.UtcNow - utcDateTime < TimeSpan.FromMinutes(60))
                {
                    Thread.Sleep(30000);
                    Console.WriteLine("sleep");
                    continue;
                }

                var newCount = matches.Count;

                if (matches.Any()) seq = matches.Max(n => n.match_seq_num);

                matches = matches.Where(n => preparedClustersId.Contains(n.cluster / 10)).ToList();
                matches = matches.Where(n => n.game_mode == 22).ToList(); //ranked ap
                matches = matches.Where(n => n.lobby_type == 7).ToList(); //ranked ap
                matches = matches.Where(n =>
                {
                    var accountIds = n.players.Select(x => x.account_id.ToString());
                    var accounts = string.Join(",", accountIds);
                    var profiles = repository.GetProfiles(accounts);
                    n.HightRankProof = profiles.Any();
                    n.NeedRankCheck = profiles.All(n => n.stats?.rank == null);
                    return n.HightRankProof;
                }).ToList();
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

        private static int _i;

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            var strings = _proxyList[_i % _keys.Count].Split(":");
            var host = strings[0];
            var port = int.Parse(strings[1]);
            var key = _keys[_i % _keys.Count];
            try
            {
                var url =
                    $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={key}";

                var webProxy = new WebProxy(host, port);
                var webClient = new WebClient
                {
                   // Proxy = webProxy
                };
                var json = webClient.DownloadString(url);
                var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
                if (matchHistory.result.status == 2)
                {
                    return null;
                }

                return matchHistory.result.matches ?? new List<MatchDetails>();
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
                _i++;
                Thread.Sleep(1000);
                return GetMatchesStartingOnSeqNumber(seqNumber);
            }
        }

        private static IList<MatchDetails> GetMatches()
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistory/V001?key={_key}&matches_requested=1";
            var webClient = new WebClient();
            var json = webClient.DownloadString(url);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
        }
    }
}