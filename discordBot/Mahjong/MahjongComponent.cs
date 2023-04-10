using Discord;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Mahjong
{
    internal class MahjongComponent
    {
        
        /// <summary>
        /// 마작 메인 임베드
        /// </summary>
        /// <returns></returns>
        public static EmbedBuilder CreateMahjongMainEmbed()
        {
            return new EmbedBuilder()
            {
                Title = "마작 어시스터",
                ThumbnailUrl = @"https://pbs.twimg.com/profile_images/1118736457554964480/_G7Au64E_400x400.png"
            };
        }

        /// <summary>
        /// 게임 정보 표시컴포넌트, 임베드
        /// </summary>
        /// <param name="gameHandler"></param>
        /// <returns></returns>
        public static (Embed embed, MessageComponent component) CreateMainGameMsg(ulong gameHandler)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";

            int playerCount = 0;

            string ton = "";
            string nan = "";
            string sha = "";
            string pe = "";

            string wind = "";
            string rounds = "";
            string extra = "";

            string gameType = "";
            string table = "";
            using (StreamReader file = File.OpenText(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject json = (JObject)JToken.ReadFrom(reader);
                    try
                    {
                        playerCount = (int)json["PlayerCount"];

                        ton = json["Ton"].ToString();
                        nan = json["Nan"].ToString();
                        sha = json["Sha"].ToString();
                        pe = json["Pe"].ToString();

                        rounds = json["Rounds"].ToString();
                        gameType = json["GameType"].ToString();
                        if (gameType == "Han")
                        {
                            gameType = "반장전";
                        }
                        else if (gameType == "Ton")
                        {
                            gameType = "동풍전";
                        }

                        extra = json["Extra"].ToString();
                        table = json["Table"].ToString();

                        switch (json["Wind"].ToString())
                        {
                            case "Ton":
                                wind = "동풍";
                                break;
                            case "Nan":
                                wind = "남풍";
                                break;
                        }
                    }
                    catch
                    {

                    }
                }
            }

            var fieldTon = new EmbedFieldBuilder()
                .WithName("동")
                .WithValue(ton)
                .WithIsInline(true);
            var fieldNan = new EmbedFieldBuilder()
                 .WithName("남")
                 .WithValue(ton)
                 .WithIsInline(true);
            var fieldTable = new EmbedFieldBuilder()
                .WithName($"{extra}연짱")
                .WithValue($"공탁{table}점")
                .WithIsInline(false);
            var fieldSha = new EmbedFieldBuilder()
                .WithName("서")
                .WithValue(ton)
                .WithIsInline(true);
            var fieldPe = new EmbedFieldBuilder()
                .WithName("북")
                .WithValue(ton)
                .WithIsInline(true);

            EmbedBuilder embedBuilder = CreateMahjongMainEmbed()
                .WithDescription($"{gameType} {wind} {rounds}국")
                .AddField(fieldTon)
                .AddField(fieldNan)
                .AddField(fieldTable)   
                .AddField(fieldSha);

            if(playerCount > 3)
            {
                embedBuilder.AddField(fieldPe);
            }

            MessageComponent component = CreateMainGameComponent(playerCount);

            return (embedBuilder.Build(), component);
        }

        public static MessageComponent CreateStartGameMenuComponent()
        {
            var playerMenuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("플레이어 수")
                .WithCustomId("mahj_player")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption("3인", "3")
                .AddOption("4인", "4");

            var styleMenuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("방식")
                .WithCustomId("mahj_style")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption("동풍전", "ton")
                .AddOption("반장전", "nan");

            var builder = new ComponentBuilder()
                .WithSelectMenu(playerMenuBuilder, 1)
                .WithSelectMenu(styleMenuBuilder, 2)
                .WithButton("확인", "mahj_startgame", row: 3)
                .AddRow(CreateBackwardComponent("start"));

            return builder.Build();
        }

        /// <summary>
        /// 동남서북 버튼 생성
        /// </summary>
        /// <param name="playerCount"></param>
        /// <returns></returns>
        private static MessageComponent CreateMainGameComponent(int playerCount)
        {
            ComponentBuilder builder = new ComponentBuilder()
                .WithButton("동", "mahj_ton")
                .WithButton("남", "mahj_nan")
                .WithButton("서", "mahj_sha");

            if(playerCount > 3)
            {
                builder.WithButton("북", "mahj_pe");
            }

            return builder.Build();
        }

        /// <summary>
        /// 이전, 처음 복귀
        /// </summary>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        private static ActionRowBuilder CreateBackwardComponent(string moveTo)
        {
            ComponentBuilder builder = new ComponentBuilder()
                .WithButton("뒤로가기", $"mahj_back:{moveTo}")
                .WithButton("처음으로", "mahj_first");
            
            return builder.ActionRows.First();
        }

        /// <summary>
        /// 플레이어 정보 표시
        /// </summary>
        /// <param name="wind"></param>
        /// <returns></returns>
        public static MessageComponent CreatePlayerMenu(string wind)
        {
            ComponentBuilder builder = new ComponentBuilder()
                .WithButton("리치", $"mahj_reach:{wind}")
                .WithButton("론", $"mahj_ron:{wind}")
                .WithButton("쯔모", $"mahj_tsumo:{wind}")
                .WithButton("쵼보", $"mahj_chon:{wind}");

            builder.AddRow(CreateBackwardComponent("main"));

            return builder.Build();
        }
    }
}
