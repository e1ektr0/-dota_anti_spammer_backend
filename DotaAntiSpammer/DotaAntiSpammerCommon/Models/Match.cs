using System;
using System.Collections.Generic;
using System.Linq;

namespace DotaAntiSpammerCommon.Models
{
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
                    heroes.Add(new Hero
                    {
                        Id = i * j + j + 1,
                        Games = rnd.Next(100),
                        PickRate = rnd.Next(10000) / 100M,
                        WinRate = rnd.Next(10000) / 100M,
                        FirstPickRate = rnd.Next(10000) / 100M,
                        LastPickRate = rnd.Next(10000) / 100M
                    });

                var player = new Player
                {
                    Heroes = heroes,
                    TotalGames = heroes.Sum(n => n.Games),
                    WinRate = rnd.Next(10000) / 100M
                };
                match.Players.Add(player);
            }

            return match;
        }

        public void Sort(List<string> getPlayerIDs)
        {
            var players = new List<Player>();
            getPlayerIDs.AddRange(Enumerable.Range(0, 10 - getPlayerIDs.Count()).Select(n => ""));
            foreach (var playerId in getPlayerIDs)
            {
                var firstOrDefault = Players.FirstOrDefault(n => n.AccountId.ToString() == playerId);
                players.Add(firstOrDefault);
            }

            Players = players;
        }
    }
}