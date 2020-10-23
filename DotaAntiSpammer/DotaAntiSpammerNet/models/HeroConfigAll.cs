using System.Collections.Generic;
using System.IO;
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
}