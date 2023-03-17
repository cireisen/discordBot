using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot
{
    internal static class Config
    {
        public static string Token = "";
        public static ulong ControlGuildID = 0;
        public static ulong ControlChannelID = 0;
        public static string path = AppDomain.CurrentDomain.BaseDirectory + "\\";

        public static void StartConfig()
        {
            using(StreamReader file = File.OpenText(path + @"resource\Config.json"))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject json = (JObject)JToken.ReadFrom(reader);
                    try
                    {

                        Token = json["Token"].ToString();
                        ControlGuildID = (ulong)json["GuildID"];
                        ControlChannelID = (ulong)json["ChannelID"];
                    }
                    catch 
                    {

                    }
                }
            }
        }
    }
}
