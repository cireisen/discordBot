using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.Arknights
{
    internal static class ToolBoxMain
    {
        public static async Task ReadCommand(SocketInteraction socket, string id)
        {
            switch(id)
            {
                case "main":
                    await ShowMain(socket);
                    break;
            }
        }

        public static async Task ShowMain(SocketInteraction command)
        {
            var embed = new EmbedBuilder()
            {
                Title = "명일방주 툴박스 입니다.",
                ThumbnailUrl = @"https://upload.wikimedia.org/wikipedia/en/a/aa/Arknights_icon.png",
                Description = "아직 개발중"
            };

            embed.AddField("원하시는 기능의 버튼을 클릭해 주세요", "응애");

            var builder = new ComponentBuilder()
                .WithButton("DPS계산기", "mrfz_dpscalc");

            if(command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = builder.Build();
                });
            }
            else if(command.GetType() == typeof(SocketSlashCommand))
            {
                await command.RespondAsync("", new Embed[] { embed.Build()}, components: builder.Build());
            }
        }
    }
}
