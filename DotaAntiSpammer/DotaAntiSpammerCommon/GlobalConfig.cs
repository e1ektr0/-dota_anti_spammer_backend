namespace DotaAntiSpammerCommon
{
    public static class GlobalConfig
    {
        public static string ApiUrl { get;  } = "http://194.87.103.72:5003";
        public static string StatsUrl { get; } = "/stats";
        public static string ConnectionString { get; } =  "mongodb://e1ekt0:secretPassword@194.87.103.72:27017/?authSource=dota2&readPreference=primary&appname=MongoDB%20Compass&ssl=false";
    }
}