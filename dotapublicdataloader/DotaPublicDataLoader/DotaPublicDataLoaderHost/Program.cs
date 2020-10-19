using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace DotaPublicDataLoaderHost
{
    // 1)Выкачивать только актуальные данные+
    // 2)рейт лимиты 
    // 3)сделать программу анализа(расчить самые пикаемы ,
    // вр и прочею херь для каждого игрока) + 10h
    // 4)сделать api для получения этих данных +2h
    // 5)написать софтину которая подключается к доте
    // и показывает результаты(как овервульф). + 40h
    // 6)нарегать дохуя стим аккаунтов(есть софт для этого)

    internal static class Program
    {
        private static string _key = "D356E276EC76CE809A433AC8A0B81952";

        //5662200710
        private static void Main()
        {
            var matchesRepository = new MatchesRepository();
            var lastSeq = matchesRepository.GetLastSeq();
            while (true)
            {
                lastSeq ??= GetSeq();
                var matches = GetMatchesStartingOnSeqNumber(lastSeq.Value + 1);
                Thread.Sleep(1000);
                if (matches.Any())
                    matchesRepository.Insert(matches);
                if(matches.Any())
                    lastSeq = matches.Max(n => n.match_seq_num);
                Thread.Sleep(1);
                Console.WriteLine(lastSeq);
            }
        }

        private static ulong GetSeq()
        {
            var matches = GetMatches();
            return matches.Any() ? matches.Max(n=>n.match_seq_num) : (ulong) 4756161920;
        }

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={_key}";
            var json = new WebClient().DownloadString(url);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
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