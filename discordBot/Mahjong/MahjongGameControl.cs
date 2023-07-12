using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace discordBot.Mahjong
{
    internal static class MahjongGameControl
    {
        internal enum WinType { Ron, Tsumo };
        private static List<string> listWind = new List<string>(new string[] { "Ton", "Nan", "Sha", "Pe" });
        /// <summary>
        /// 점수 이동
        /// </summary>
        /// <param name="id"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="point"></param>
        public static void MovePoint(ulong id, string wind, string target, int point)
        {
            string path = Config.path;
            string filePath = path + @"resource\Config.json";
            string jsonString = "";
            using (StreamReader file = File.OpenText(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject json = (JObject)JToken.ReadFrom(reader);
                    try
                    {
                        int mainPoint = (int)   json[wind];
                        int targetPoint = (int)json[target];

                        mainPoint += point;
                        targetPoint += point;

                        json[wind] = mainPoint;
                        json[target] = targetPoint;

                        jsonString = json.ToString();
                    }
                    catch
                    {

                    }

                    
                }
            }

            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(jsonString);
            }
        }

        /// <summary>
        /// 론 게임 동작
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wind"></param>
        /// <param name="target"></param>
        /// <param name="pan"></param>
        /// <param name="fu"></param>
        public static void PlayerRon(ulong gameHandler, string wind, string target, int playerCount, int pan, int fu)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";
            string jsonString = "";

            int mainPoint = 0;
            int targetPoint = 0;

            try
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {

                        JObject json = (JObject)JToken.ReadFrom(reader);
                        try
                        {
                            mainPoint = (int)json[wind];
                            targetPoint = (int)json[target];

                            int point = PointCalculator.CalcPoint(pan, fu, playerCount, wind == "Ton", WinType.Ron);

                            mainPoint += point;
                            targetPoint -= point;

                            json[wind] = mainPoint;
                            json[target] = targetPoint;

                            jsonString = json.ToString();
                        }
                        catch
                        {

                        }
                    }
                }
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                
            }
        }

        /// <summary>
        /// 쯔모시 게임 동작
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wind"></param>
        /// <param name="pan"></param>
        /// <param name="fu"></param>
        public static void PlayerTsumo(ulong gameHandler, string wind, int playerCount, int pan, int fu)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";
            string jsonString = "";

            int mainPoint = 0;
            int targetPoint = 0;

            try
            {


                using (StreamReader file = File.OpenText(filePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        int point = PointCalculator.CalcPoint(pan, fu, playerCount, wind == "Ton", WinType.Ron);
                        JObject json = (JObject)JToken.ReadFrom(reader);
                        try
                        {
                            for (int i = 0; i < playerCount; i++)
                            {
                                string target = listWind[i];
                                if(wind == target)
                                {
                                    continue;
                                }
                                mainPoint = (int)json[wind];
                                targetPoint = (int)json[target];



                                mainPoint += point;
                                targetPoint -= point;

                                json[wind] = mainPoint;
                                json[target] = targetPoint;


                                jsonString = json.ToString();
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 오야 변경
        /// </summary>
        public static void ChangeWind(ulong id)
        {

        }

        /// <summary>
        /// 유국
        /// </summary>
        /// <param name="tenpai"></param>
        public static void DrawRound(ulong id, List<string> tenpai)
        {
            switch(tenpai.Count)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        public  static void ChangeGameOption(ulong gameHandler, string option, object value)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";

            try
            {
                string jsonString = "";
                using (StreamReader file = File.OpenText(filePath))
                {
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {

                        JObject json = (JObject)JToken.ReadFrom(reader);
                        try
                        {
                            json[option] = JToken.FromObject(value);
                        }
                        catch
                        {

                        }
                        jsonString = json.ToString();

                    }
                }
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mahjong Change Option Error.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
