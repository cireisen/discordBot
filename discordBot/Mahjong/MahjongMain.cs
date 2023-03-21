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
            var embed = MahjongComponent.CreateMahjongMainEmbed()
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
                var messageInfo = MahjongComponent.CreateMainGameMsg(userId);
                await command.RespondAsync("", embed: messageInfo.embed, ephemeral: false, components: messageInfo.component);
            }
        }

        public static async Task StartGame(SocketInteraction command, long count)
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

                CreateGame(userId, count);

                var gameinfo = MahjongComponent.CreateMainGameMsg(userId);

                await command.RespondAsync("", embed: gameinfo.embed, ephemeral: false, components: gameinfo.component);
            }
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
