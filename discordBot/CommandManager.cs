using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using discordBot.Arknights;
using discordBot.Mahjong;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot
{
    internal class CommandManager
    {
        private readonly DiscordSocketClient _client;
        public CommandManager(DiscordSocketClient client) 
        {
            _client = client;
        }

        public async void CreateSlashCommands()
        {
            var globalCommand = new SlashCommandBuilder();
            
            globalCommand.WithName("first-global-command");
            globalCommand.WithDescription("This is my first global command");

            var buttonCommand = new SlashCommandBuilder();

            buttonCommand.WithName("buttontest");
            buttonCommand.WithDescription("Button Test");

            var menuCommand = new SlashCommandBuilder();

            menuCommand.WithName("createmenu");
            menuCommand.WithDescription("CreateMainMenu");

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(buttonCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(menuCommand.Build());
            }
            catch (ApplicationCommandException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                var json = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);

                Console.WriteLine(json.ToString());
            }

            
        }

        public async void SetCommandAction()
        {
            _client.SlashCommandExecuted += client_SlashCommandExecuted;
            _client.ButtonExecuted += client_ButtonExecuted;
            _client.SelectMenuExecuted += _client_SelectMenuExecuted;
        }

        private async Task _client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            switch (arg.Data.CustomId)
            {
                case "menu1":
                    var value = arg.Data.Values.First();
                    var menu = new SelectMenuBuilder()
                    {
                        CustomId = "menu1",
                        Placeholder = $"{(arg.Message.Components.First().Components.First() as SelectMenuComponent).Options.FirstOrDefault(x => x.Value == value).Label}",
                        MaxValues = 1,
                        MinValues = 1
                    };

                    menu.AddOption("Meh", "1", "Its not gaming.")
                        .AddOption("Ish", "2", "Some would say that this is gaming.")
                        .AddOption("Moderate", "3", "It could pass as gaming")
                        .AddOption("Confirmed", "4", "We are gaming")
                        .AddOption("Excellent", "5", "It is renowned as gaming nation wide", new Emoji("🔥"));

                    //We use UpdateAsync to update the message and its original content and components.
                    await arg.UpdateAsync(x =>
                    {
                        x.Content = $"Thank you {arg.User.Mention} for rating us {value}/5 on the gaming scale";
                        x.Components = new ComponentBuilder().WithSelectMenu(menu).Build();
                    });

                    var embed = new EmbedBuilder();

                    var buttons = new EmbedBuilder();

                    //await arg.UpdateAsync(x =>
                    //{
                        
                    //});
                    break;
            }
        }

        private async Task client_ButtonExecuted(SocketMessageComponent arg)
        {
            string id = arg.Data.CustomId;

            var info = id.Split('_');

            //SocketInteraction a = arg;

            switch (info[0])
            {
                case "main":
                    break;
                    //방주
                case "mrfz":
                    await ToolBoxMain.ReadCommand(arg, info[1]);
                    break;
                    //마작
                case "mahj":
                    await MahjongMain.ReadCommand(arg, info[1]);
                    break;

            }

            await arg.RespondAsync(arg.Data.CustomId);
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand command)
        {
            string name = command.CommandName;

            var commandData= name.Split('_');

            switch(commandData[0])
            {
                case "first-global-command":
                    await command.RespondAsync($"You executed {command.Data.Name}");
                    break;
                case "buttontest":
                    await ButtonTest(command);
                    break;
                case "createmenu":
                    await CreateMenu(command);
                    break;
                case "mrfz":
                    await ToolBoxMain.ReadCommand(command, commandData[1]);
                    break;
                case "mahj":
                    await MahjongMain.ReadCommand(command, commandData[1]);
                    break;
            }

        }

        public async Task ButtonTest(SocketSlashCommand command)
        {
            var builder = new ComponentBuilder()
                .WithButton("label", "custom-id");

            await command.RespondAsync("Here is a button!", components: builder.Build());
        }

        public async Task CreateMenu(SocketSlashCommand command)
        {
            var embed = new EmbedBuilder()
            {
                Title = "우동게 봇 메인입니다.",
                ThumbnailUrl = @"https://avatars.githubusercontent.com/u/28528895?v=4",
                Description = "아직 개발중"
            };

            embed.AddField("원하시는 기능을 선택하세요", "응애");

            var builder = new ComponentBuilder()
                .WithButton("명일방주", "mrfz_main")
                .WithButton("쓰레기게임", "trhg_main");
            

            await command.RespondAsync(embed:  embed.Build() , components: builder.Build());
        }

        
    }
}
