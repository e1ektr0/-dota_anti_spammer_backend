﻿namespace DotaAntiSpammerCommon
{
    public static class GlobalConfig
    {
        public static string ApiUrl { get;  } = "http://localhost:5003";
        public static string StatsUrl { get; } = "/stats";
        public static string ConnectionString { get; } =  "mongodb://194.87.103.72:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false";
    }
}