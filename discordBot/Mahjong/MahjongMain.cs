using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

                    break;
            }
        }

        public static async Task ShowMain(SocketInteraction command)
        {
            var embed = new EmbedBuilder()
            {
                Title = "마작 어시스터 입니다.",
                ThumbnailUrl = @"https://upload.wikimedia.org/wikipedia/en/a/aa/Arknights_icon.png",
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

            string filePath = System.Reflection.Assembly.GetExecutingAssembly().Location + @$"\mahjong\{userId}";
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                //JsonWriter json = new JsonWriter();
                //json.


                await command.RespondAsync("");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mahjong StartGame Error.");
                Console.WriteLine(ex.Message);
            }
        }

        public static Embed CreateMainGameEmbed(ulong gameHandler)
        {


            return new EmbedBuilder().Build();
        }
    }
}
