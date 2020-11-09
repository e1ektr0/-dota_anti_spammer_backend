using System.Collections.Generic;
using System.Linq;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerMongo.Models;

namespace DotaAntiSpammerMongo
{
    public class Calculator
    {
        public static Player Calculate(long accountId, List<PlayerResult> results)
        {
            var grouping = results.GroupBy(n => n.match_id).Select(n => n.FirstOrDefault()).ToList();
            var player = new Player {AccountId = accountId, Heroes = new List<Hero>()};
            var totalGames = grouping.Count();
            player.TotalGames = totalGames;
            player.WinRate = grouping.Count(n => n.win) * 100 / (decimal) totalGames;
            foreach (var heroStats in grouping.GroupBy(n => n.hero_id))
            {
                var playerResults = heroStats.GroupBy(n => n.match_id).Select(n => n.FirstOrDefault()).ToList();
                var hero = new Hero {Id = heroStats.Key, Games = playerResults.Count()};
                hero.WinRate = (decimal) playerResults.Count(n => n.win) * 100 / hero.Games;
                var gamesHeroBanned = grouping.Count(n => n.bans.Any(u => u == heroStats.Key));
                var totalWithoutBans = totalGames - gamesHeroBanned;
                if (totalWithoutBans == 0)
                {
                    totalWithoutBans = playerResults.Count();
                }

                hero.PickRate = (decimal) playerResults.Count() * 100 / totalWithoutBans;
                var firstPickCount = playerResults.Count(n => n.hero_pick_order <= 4);
                hero.FirstPickRate = ((decimal) firstPickCount / hero.Games) * 100;
                var lastPickCount = playerResults.Count(n => n.hero_pick_order >= 9 && n.hero_pick_order <= 10) *
                                    100;
                hero.LastPickRate = (decimal) lastPickCount / hero.Games;
                player.Heroes.Add(hero);
            }

            player.Heroes = player.Heroes.OrderByDescending(n => n.Games + n.WinRate / 100M).ToList();
            return player;
        }
    }
}