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
using DiscordBotsList.Api.Internal;
using System.Security.AccessControl;
using Discord.Interactions;

namespace discordBot.Mahjong
{
    public class MahjongMain : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService SlashCommands { get; set; }
        private CommandManager _manager;
        public MahjongMain(CommandManager manager)
        {
            _manager = manager;
        }
        /// <summary>
        /// 마작 커맨드 읽기 구분_동작:데이터
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task ReadCommand(SocketInteraction socket, string id)
        {
            string value = "";

            switch (id.Split(':')[0])
            {
                case "main":
                    await ShowMain(socket);
                    break;
                case "startmenu":
                    await CreateMain(socket);
                    break;
                case "startgame":
                    await StartGame(socket);
                    break;
                case "ton":
                case "nan":
                case "sha":
                case "pe":
                    await ShowPlayerMenu(socket, id);
                    break;
                case "back":
                    string direction = id.Split(':')[1];
                    await ChangeMessage(socket, direction);
                    break;
                case "ron":
                    value = id.Split(":")[1];
                    await PlayerRon(socket, value);
                    break;
                case "tsumo":
                    value = id.Split(":")[1];
                    await PlayerTsumo(socket, value);
                    break;
            }
        }

        /// <summary>
        /// Mahj Selectmenu ValueChanged
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task SelectMenuExecute(SocketMessageComponent socket, string id)
        {
            switch (id.Split(':')[0])
            {
                case "player":
                    
                    MahjongGameControl.ChangeGameOption(socket.User.Id, "PlayerCount", long.Parse(socket.Data.Values.First()));
                    break;
                case "style":
                    MahjongGameControl.ChangeGameOption(socket.User.Id, "GameStyle", socket.Data.Values.First());
                    break;
            }
        }

        public static async Task ModalSubmitted(SocketModal socket, string id)
        {
            switch (id.Split(':')[0])
            {
                case "ron":

                    break;
                case "tsumo":
                    
                    break;
            }
        }

        /// <summary>
        /// Show MahjongMenu
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static async Task ShowMain(SocketInteraction command)
        {
            var embed = MahjongComponent.CreateMahjongMainEmbed()
                .WithDescription("아직 개발중");

            embed.AddField("원하시는 기능의 버튼을 클릭해 주세요", "응애");

            var builder = new ComponentBuilder()
                .WithButton("게임시작", "mahj_startmenu");

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

        /// <summary>
        /// Show StartMahj Options
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static async Task CreateMain(SocketInteraction command)
        {
            CreateGame(command.User.Id, 3, "han");
            var embed = MahjongComponent.CreateMahjongMainEmbed()
                .WithDescription("인원수와 방식을 선택해주세요.");

            var component = MahjongComponent.CreateStartGameMenuComponent();

            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = component;
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                await command.RespondAsync("", new Embed[] { embed.Build() }, components: component, ephemeral: true);
            }
        }

        /// <summary>
        /// CreateGame
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static async Task StartGame(SocketInteraction command)
        {
            ulong userId = command.User.Id;
            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                
                var components = msg.Message.Components.ToList();

                string a = (components[0].Components.First() as SelectMenuComponent).ToString();
                var s = msg.GetOriginalResponseAsync();

                _ = a;
                await msg.UpdateAsync(x =>
                {
                    //x.Embed = embed.Build();
                    //x.Components = builder.Build();
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                SocketSlashCommand slash = (SocketSlashCommand)command;

                var values = slash.Data.Options.ToDictionary(data => data.Name);
                long count = (long)values["playercount"].Value;
                string style = (string)values["gamestyle"].Value;

                CreateGame(userId, count, style);
                var messageInfo = MahjongComponent.CreateMainGameMsg(userId);
                await command.RespondAsync("", embed: messageInfo.embed, ephemeral: false, components: messageInfo.component);
            }
        }

        private static async Task ShowPlayerMenu(SocketInteraction command, string wind)
        {
            var messageComp = MahjongComponent.CreatePlayerMenu(wind);

            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    //x.Embed = embed.Build();
                    x.Components = messageComp;
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                SocketSlashCommand slash = (SocketSlashCommand)command;
                

                await command.RespondAsync("", ephemeral: false, components: messageComp);
            }
        }

        private static async Task ShowMainGame(SocketInteraction command)
        {
            ulong userId = command.User.Id;
            var messageComp = MahjongComponent.CreateMainGameMsg(userId);
            if (command.GetType() == typeof(SocketMessageComponent))
            {
                SocketMessageComponent msg = (SocketMessageComponent)command;
                await msg.UpdateAsync(x =>
                {
                    x.Embed = messageComp.embed;
                    x.Components = messageComp.component;
                });
            }
            else if (command.GetType() == typeof(SocketSlashCommand))
            {
                SocketSlashCommand slash = (SocketSlashCommand)command;


                await command.RespondAsync("", ephemeral: false, components: messageComp.component);
            }

        }

        private static async Task ChangeMessage(SocketInteraction command, string direction)
        {
            switch (direction)
            {
                case "start":
                    await ShowMain(command);
                    break;
                case "main":
                    await ShowMainGame(command);
                    break;
            }

        }

        private static async Task PlayerRon(SocketInteraction command, string ronPlayer)
        {
            var modal = MahjongComponent.CreateWinModal("ron_" + ronPlayer);

            command.RespondWithModalAsync(modal);
        }

        private static async Task PlayerTsumo(SocketInteraction command, string winPlayer)
        {
            var modal = MahjongComponent.CreateWinModal("tsumo");

            command.RespondWithModalAsync(modal);
        }

        private static void CreateGame(ulong gameHandler, long playerCount, string gameType)
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
                    new JProperty("GameType", gameType), //동풍 "Ton", 반장"Han"
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
