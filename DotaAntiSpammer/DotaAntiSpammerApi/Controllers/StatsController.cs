using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        public Match Get(string accounts, long currentId)
        {
            var enumerable = accounts.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse).ToList();

            var results = _repository.GetPlayers(accounts);

            var result = new Match
            {
                Players = results.Select(n => n.stats).Where(n => n != null).ToList()
            };

            return result;
        }

        [HttpPost("Wards")]
        public List<PlayerWards> Wards(long currentId, [FromBody] List<PlayerPick> picks)
        {
//            var list = _repository.GetWards(105, true)
//                .GroupBy(n=>n.account_id).Select(n=>new {n.Key, c =n.Count()}).OrderByDescending(n=>n.c).ToList().FirstOrDefault();
            var result = picks.Select(n => new PlayerWards
            {
                Wards = _repository.GetWards(n.AccountId, n.HeroId, n.Radiant)
                    .SelectMany(x => x.results.Select(u =>
                        new WardPlaced
                        {
                            Obs = u.obs,
                            Time = u.time,
                            X = u.x,
                            Y = u.y,
                            VecX = u.vecx,
                            VecY = u.vecy,
                            Mine = u.mine,
                            MatchId = x.match_id
                        })).ToList(),
                AccountId = n.AccountId,
                HeroId = n.HeroId
            }).ToList();

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

        [HttpGet]
        [Route("player")]
        public string GetMatches(int accountId)
        {
            var results = _repository.GetResultsByAccountId(accountId);

            var matches = results.OrderByDescending(n => n.startTime).Select(n =>
                DateTimeOffset.FromUnixTimeSeconds((long) n.startTime).ToString("g") +
                (n.win ? " win " : " lose ") + HeroConfigAll.Instance.Heroes.First(x => x.Id == n.hero_id).Name +
                " https://www.dotabuff.com/matches/" + n.match_id + "\r\n").ToList();
            return string.Join(Environment.NewLine, matches);
        }
    }

    public class HeroConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalName { get; set; }
    }

    public class HeroConfigAll
    {
        private static HeroConfigAll _instance;
        public List<HeroConfig> Heroes { get; set; }

        public static HeroConfigAll Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                var readAllText = File.ReadAllText("heroes.txt");
                _instance = JsonSerializer.Deserialize<HeroConfigAll>(readAllText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return _instance;
            }
        }
    }
}