using System;
using System.Collections.Generic;
using System.Linq;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerMongo;
using Microsoft.AspNetCore.Mvc;

namespace DotaAntiSpammerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly MongoRepository _repository;

        public StatsController()
        {
            _repository = new MongoRepository();
        }

        [HttpGet]
        public Match Get(string accounts)
        {
            //todo: accounts sql inj
            var results = _repository.GetResults(accounts);

            var result = new Match
            {
                Players = new List<Player>(),
            };

            foreach (var playerStats in results.GroupBy(n => n.account_id))
            {
                var player = new Player {AccountId = playerStats.Key, Heroes = new List<Hero>()};
                var totalGames = playerStats.Count();
                player.TotalGames = totalGames;
                player.WinRate = playerStats.Count(n => n.win) * 100 / (decimal) totalGames;
                result.Players.Add(player);
                foreach (var heroStats in playerStats.GroupBy(n => n.hero_id))
                {
                    var playerResults = heroStats.GroupBy(n => n.match_id).Select(n => n.FirstOrDefault()).ToList();
                    var hero = new Hero {Id = heroStats.Key, Games = playerResults.Count()};
                    hero.WinRate = (decimal) playerResults.Count(n => n.win) * 100 / hero.Games;
                    var gamesHeroBanned = playerStats.Count(n => n.bans.Any(u => u == heroStats.Key));
                    hero.PickRate = (decimal) playerResults.Count() * 100 / (totalGames - gamesHeroBanned);
                    var firstPickCount = playerResults.Count(n => n.hero_pick_order <= 4);
                    hero.FirstPickRate = ((decimal) firstPickCount / hero.Games) * 100;
                    var lastPickCount = playerResults.Count(n => n.hero_pick_order >= 9 && n.hero_pick_order <= 10) *
                                        100;
                    hero.LastPickRate = (decimal) lastPickCount / hero.Games;
                    player.Heroes.Add(hero);
                }

                player.Heroes = player.Heroes.OrderByDescending(n => n.Games+n.WinRate/100M).ToList();
            }
            return result;
        }
        
        [HttpGet]
        [Route("default")]
        public Match GetDefault()
        {
            return Match.Sample();
        }


        [HttpGet]
        [Route("top")]
        public List<string> GetTop(int heroId)
        {
            var resultsByHeroId = _repository.GetResultsByHeroId(heroId);
            var groupBy = resultsByHeroId.GroupBy(n => n.account_id);
            var enumerable = groupBy.OrderByDescending(n => n.Count()).Take(10);
            return enumerable.Select(n => "https://www.dotabuff.com/players/" + n.Key).ToList();
        }

        [HttpGet]
        [Route("all_top")]
        public List<string> GetTopAll()
        {
            var result = new List<string>();
            for (int i = 0; i < 130; i++)
            {
                var resultsByHeroId = _repository.GetResultsByHeroId(i);
                var groupBy = resultsByHeroId.GroupBy(n => n.account_id);
                var enumerable = groupBy.OrderByDescending(n => n.Count()).Take(1);
                var @select = enumerable.Select(n => "https://www.dotabuff.com/players/" + n.Key);
                if (select.Any())
                    result.Add(@select.First());
            }

            return result;
        }


        [HttpGet]
        [Route("games")]
        public List<string> GetMatches(int accountId, int heroId)
        {
            var results = _repository.GetResultsByAccountId(accountId, heroId);

            return results.Select(n => "https://www.dotabuff.com/matches/" + n.match_id).ToList();
        }
    }
}