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
                        int mainPoint = (int)json[wind];
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
        public static void PlayerRon(ulong id, string wind, string target, int pan, int fu)
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
                        int mainPoint = (int)json[wind];
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
        /// 쯔모시 게임 동작
        /// </summary>
        /// <param name="id"></param>
        /// <param name="wind"></param>
        /// <param name="pan"></param>
        /// <param name="fu"></param>
        public static void PlayerTsumo(ulong id, string wind, int pan, int fu)
        {

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
    }
}
