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

namespace discordBot.Mahjong
{
    internal class MahjongMain
    {
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
            var embed = new EmbedBuilder()
            {
                Title = "마작 어시스터 입니다.",
                ThumbnailUrl = @"https://pbs.twimg.com/profile_images/1118736457554964480/_G7Au64E_400x400.png",
                Description = "아직 개발중"
            };

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

                int count = (int)slash.Data.Options.First().Value;

                CreateGame(userId, count);

                await command.RespondAsync("", embed:CreateMainGameEmbed(userId), ephemeral: true);
            }
        }

        public static Embed CreateMainGameEmbed(ulong gameHandler)
        {
            string filePath = Config.path + @$"mahjong\{gameHandler}.json";

            string ton = "";
            string nan = "";
            string sha = "";
            string pe = "";

            using (StreamReader file = File.OpenText(filePath))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {

                    JObject json = (JObject)JToken.ReadFrom(reader);
                    try
                    {

                        ton = json["ton"].ToString();
                        nan = json["nan"].ToString();
                        sha = json["sha"].ToString();
                        pe  = json["pe"].ToString();
                        
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
                .WithName("0연짱")
                .WithValue("공탁0점")
                .WithIsInline(false);
            var fieldSha = new EmbedFieldBuilder()
                .WithName("서")
                .WithValue(ton)
                .WithIsInline(true);
            var fieldPe = new EmbedFieldBuilder()
                .WithName("북")
                .WithValue(ton)
                .WithIsInline(true);

            EmbedBuilder embedBuilder = new EmbedBuilder().WithFields(new EmbedFieldBuilder[] { fieldTon, fieldPe, fieldTable, fieldNan, fieldSha });

            return embedBuilder.Build();
        }

        private static void CreateGame(ulong gameHandler, int playerCount)
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
                    new JProperty("GameType", "han"), //동풍 "ton", 반장"han"
                    new JProperty("Rounds", 1), //국
                    new JProperty("Extra", 0), //본장
                    new JProperty("ton", 25000),
                    new JProperty("nan", 25000),
                    new JProperty("sha", 25000),
                    new JProperty("pe", 25000)

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
