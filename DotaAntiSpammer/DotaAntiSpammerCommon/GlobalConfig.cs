using System;

namespace DotaAntiSpammerCommon
{
    public static class GlobalConfig
    {
        public static string ApiUrl { get;  } = "http://"+GetIp()+":5003";

        private static string GetIp()
        {
            var ip = Environment.GetEnvironmentVariable("dota_server_ip");
            return ip??"194.87.103.72";
        }
        
        private static string GetMongoCreds()
        {
            return "e1ekt0:secretPassword";
        }

        public static string StatsUrl { get; } = "/stats";
        public static string ConnectionString { get; } =  "mongodb://"+GetMongoCreds()+"@"+GetIp()+":27017/?authSource=dota2&readPreference=primary&appname=MongoDB%20Compass&ssl=false";
    }
}