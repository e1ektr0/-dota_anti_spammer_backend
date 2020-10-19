using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;

namespace DotaPublicDataLoaderHost
{
    
    // 1)работу с офф публичными матчами(скачать их)+
    // 2)выкачать приватные данные по матчам которые нас
    // интересуют(хочу пока китай задоджить и только для европпы качать,
    // не уверен что полуится. посмотрим) + 10h
    // 3)сделать программу анализа(расчить самые пикаемы ,
    // вр и прочею херь для каждого игрока) + 10h
    
    // 4)сделать api для получения этих данных +2h
    
    // 5)написать софтину которая подключается к доте
    // и показывает результаты(как овервульф). + 40h
    
    // 6)нарегать дохуя стим аккаунтов(есть софт для этого)
    // 5h
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
                var matches = GetMatchesStartingOnSeqNumber(lastSeq + 1);

                var max = matches.Max(n=>n.start_time+(ulong)n.duration) -  matches.Min(n=>n.start_time+(ulong)n.duration);
                Console.WriteLine(matches.Count);
                Console.WriteLine(max/60);
                if (matches.Any())
                    matchesRepository.Insert(matches);
                lastSeq = matches.Max(n => n.match_seq_num);
                Thread.Sleep(1);
                Console.WriteLine(lastSeq);
            }
        }

        private static IList<MatchDetails> GetMatchesStartingOnSeqNumber(ulong seqNumber)
        {
            var url =
                $"https://api.steampowered.com/IDOTA2Match_570/GetMatchHistoryBySequenceNum/v1?start_at_match_seq_num={seqNumber}&key={_key}";
            var json = new WebClient().DownloadString(url);
            var matchHistory = JsonSerializer.Deserialize<MatchHistoryBySequenceNum>(json);
            return matchHistory.result.matches;
        }
    }
}