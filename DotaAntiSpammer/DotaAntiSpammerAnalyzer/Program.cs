using System;
using System.Linq;
using System.Threading;
using DotaAntiSpammerMongo;

namespace DotaAntiSpammerAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongoRepository = new MongoRepository();
            while (true)
            {
                var allAccountId = mongoRepository.ChangedResults().ToList();
                for (var i = 0; i < allAccountId.Count; i++)
                {
                    var acc = allAccountId[i];
                    var playerResults = mongoRepository.GetResultsByAccountId(acc.account_id);
                    var startTime = DateTime.UtcNow.AddDays(-30);
                    
                    var filterByDate = playerResults.Where(n =>
                        DateTimeOffset.FromUnixTimeSeconds((long) n.startTime).ToUniversalTime() > startTime).ToList();
                    
                    if (filterByDate.Count > 100)
                        playerResults = filterByDate;
                    var calculate = Calculator.Calculate(acc.account_id, playerResults);
                    calculate.rank = acc.stats?.rank;
                    mongoRepository.UpdateResult(calculate, playerResults.Max(n => n.match_seq_num));
                    if (filterByDate.Count > 100)
                        mongoRepository.RemoveResult(playerResults.Where(n =>
                                DateTimeOffset.FromUnixTimeSeconds((long) n.startTime).ToUniversalTime() < startTime)
                            .ToList());
                    if (i % 100 != 0)
                        continue;
                    Console.WriteLine($"{i}/{allAccountId.Count}");
                }

                Console.WriteLine("done " + allAccountId.Count);

                Thread.Sleep(1000);
            }
        }
    }
}