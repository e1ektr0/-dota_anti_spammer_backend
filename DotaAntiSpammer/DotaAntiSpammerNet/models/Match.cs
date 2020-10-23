using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DotaAntiSpammerNet.models
{
    public class HeroConfigAll
    {
        public List<HeroConfig> Heroes { get; set; }
        private static HeroConfigAll _instance;
        public static HeroConfigAll Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                var readAllText = File.ReadAllText("heroes.json");
                _instance = JsonSerializer.Deserialize<HeroConfigAll>(readAllText , new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                });
                return _instance;
            }
        }
    }
    public class HeroConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LocalName { get; set; }
    }
    public class Match
    {
        public List<Player> Players { get; set; }

        public static Match Sample()
        {
            var rnd = new Random();
            var match = new Match
            {
                Players = new List<Player>()
            };
            for (var i = 0; i < 10; i++)
            {
                var heroes = new List<Hero>();
                for (var j = 1; j < 10; j++)
                {
                    heroes.Add(new Hero
                    {
                        Id = i * j + j + 1,
                        Games = rnd.Next(100),
                        PickRate = rnd.Next(10000) / 100M,
                        WinRate = rnd.Next(10000) / 100M,
                        FirstPickRate = rnd.Next(10000) / 100M,
                        LastPickRate = rnd.Next(10000) / 100M,
                    });
                }

                var player = new Player
                {
                    Heroes = heroes,
                    TotalGames = heroes.Sum(n=>n.Games),
                    WinRate = rnd.Next(10000) / 100M
                };
                match.Players.Add(player);
            }

            return match;
        }
    }

    public class Player
    {
        public List<Hero> Heroes { get; set; }
        public int TotalGames { get; set; }
        public decimal WinRate { get; set; }
    }

    public class Hero
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public decimal WinRate { get; set; }
        public decimal PickRate { get; set; }
        public decimal FirstPickRate { get; set; }
        public decimal LastPickRate { get; set; }
    }
}