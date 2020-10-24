namespace DotaAntiSpammerCommon
{
    public static class GlobalConfig
    {
        public static string ApiUrl { get;  } = "http://localhost:666";
        public static string StatsUrl { get; } = "/stats";
        public static string ConnectionString { get; } =  "mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false";
    }
}