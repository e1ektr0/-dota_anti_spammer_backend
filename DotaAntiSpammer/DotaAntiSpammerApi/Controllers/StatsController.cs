using System.Collections.Generic;
using System.Linq;
using DotaAntiSpammerCommon.Models;
using DotaAntiSpammerMongo;
using DotaAntiSpammerMongo.Models;
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
            var results = _repository.GetPlayers(accounts);

            var result = new Match
            {
                Players = results.Select(n=>n.stats).Where(n=>n != null).ToList()
            };

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