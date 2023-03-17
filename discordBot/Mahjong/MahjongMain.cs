using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.Emit;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;

namespace discordBot.Mahjong
{
    internal class MahjongMain
    {
        #region Private

        private static EmbedBuilder CreateMahjongMainEmbed()
        {
            return  new EmbedBuilder()
            {
                Title = "마작 어시스터",
                ThumbnailUrl = @"https://pbs.twimg.com/profile_images/1118736457554964480/_G7Au64E_400x400.png"
            };
        }

        #endregion

        public static async Task ReadCommand(SocketInteraction socket, string id)
        {
            switch (id)
            {
                case "main":
                    await ShowMain(socket);
                    break;
                case "startgame":
                    await StartGame(socket);
                    break;
            }
        }

        public static async Task ShowMain(SocketInteraction command)
        {
            var embed = CreateMahjongMainEmbed()
                .WithDescription("아직 개발중");

            embed.AddField("원하시는 기능의 버튼을 클릭해 주세요", "응애");

            var builder = new ComponentBuilder()
                .WithButton("게임시작", "mahj_startgame");

            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = builder.Build();
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                await command.RespondAsync("", new Embed[] { embed.Build() }, components: builder.Build(), ephemeral: true);
            }
        }

        public static async Task StartGame(SocketInteraction command)
        {
            ulong userId = command.User.Id;
            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    //x.Embed = embed.Build();
                    //x.Components = builder.Build();
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                SocketSlashCommand slash = (SocketSlashCommand)command;

                long count = (long)slash.Data.Options.First().Value;

                CreateGame(userId, count);

                await command.RespondAsync("", embed:CreateMainGameEmbed(userId), ephemeral: false);
            }
        }

        public static Embed CreateMainGameEmbed(ulong gameHandler)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";

            int playerCount;

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
                        pe  = json["Pe"].ToString();

                        rounds = json["Rounds"].ToString();
                        gameType = json["GameType"].ToString();
                        if(gameType == "Han")
                        {
                            gameType = "반장전";
                        }
                        else if(gameType == "Ton")
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

            //EmbedBuilder embedBuilder = new EmbedBuilder()
            //{
            //    Title = "마작 어시스터",
            //    ThumbnailUrl = @"https://pbs.twimg.com/profile_images/1118736457554964480/_G7Au64E_400x400.png",
            //    Fields = new List<EmbedFieldBuilder> { fieldTon, fieldPe, fieldTable, fieldNan, fieldSha }

            //};

            EmbedBuilder embedBuilder = CreateMahjongMainEmbed()
                .WithDescription($"{gameType} {wind} {rounds}국")
                .AddField(fieldTon)
                .AddField(fieldNan)
                .AddField(fieldTable)
                .AddField(fieldSha)
                .AddField(fieldPe)
                .WithFooter($"");

            return embedBuilder.Build();
        }

        private static void CreateGame(ulong gameHandler, long playerCount)
        {

            string filePath = Config.path + @$"mahjong\{gameHandler}.json";
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }


                JObject json = new JObject()
                {
                    new JProperty("PlayerCount", playerCount),
                    new JProperty("GameType", "Han"), //동풍 "Ton", 반장"Han"
                    new JProperty("Rounds", 1), //국
                    new JProperty("Extra", 0), //본장
                    new JProperty("Table", 0),
                    new JProperty("Wind", "Ton"),
                    new JProperty("Ton", 25000),
                    new JProperty("Nan", 25000),
                    new JProperty("Sha", 25000),
                    new JProperty("Pe", 25000)

                };

                File.WriteAllText(filePath, json.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mahjong StartGame Error.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
