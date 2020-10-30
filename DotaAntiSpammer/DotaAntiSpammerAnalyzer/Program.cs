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
            //todo: get only changed accounts
            while (true)
            {
                var allAccountId =mongoRepository.ChangedResults().Select(n=>n.account_id).ToList();
                for (var i = 0; i < allAccountId.Count; i++)
                {
                    var acc = allAccountId[i];
                    var playerResults = mongoRepository.GetResultsByAccountId(acc);
                    var calculate = Calculator.Calculate(acc, playerResults);
                    mongoRepository.UpdateResult(calculate, playerResults.Max(n=>n.match_seq_num));
                    var newValue = ((i)/allAccountId.Count )*100;
                    if (i%100 != 0) 
                        continue;
                    Console.WriteLine($"{i}/{allAccountId.Count}");
                }
                
                Thread.Sleep(1000);
            }
            
        }
    }
}