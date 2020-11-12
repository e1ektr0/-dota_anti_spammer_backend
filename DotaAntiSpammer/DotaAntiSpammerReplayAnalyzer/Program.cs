using System;
using System.Net;
using DotaAntiSpammerMongo;

namespace DotaAntiSpammerReplayAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var mongoRepository = new MongoRepository();
            var matchForAnalyze = mongoRepository.GetMatchForAnalyze();
            foreach (var match in matchForAnalyze)
            {
                var matchName = $"{match.match_id}_{match.replay_salt}.dem.bz2";
                var url = $"http://replay{match.cluster}.valve.net/570/" + matchName;
                new WebClient().DownloadFile(url, matchName);
            }
        }
    }
}