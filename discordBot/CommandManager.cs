﻿using Discord;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace discordBot
{
    public class CommandManager
    {
        private readonly InteractionService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        public CommandManager(DiscordSocketClient client, InteractionService commands, IServiceProvider services) 
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeCommand()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            CreateSlashCommands();
            SetCommandAction();
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

            var mahjongCommand = new SlashCommandBuilder();

            mahjongCommand.WithName("startgame");
            mahjongCommand.WithDescription("마작 게임 시작");

            mahjongCommand
                .AddOption(new SlashCommandOptionBuilder()
                .WithName("playercount")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithDescription("플레이어수")
                .WithRequired(true)
                .WithMinValue(3)
                .WithMaxValue(4)
            );

            mahjongCommand
                .AddOption(new SlashCommandOptionBuilder()
                .WithName("gamestyle")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("게임 종류")
                .WithRequired(true)
                .AddChoice("동풍전", "ton")
                .AddChoice("반장전", "han")
            );

            

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(buttonCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(menuCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(mahjongCommand.Build());
            }
            catch (HttpException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                var json = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);

                Console.WriteLine(json.ToString());
            }

            
        }

        public async void SetCommandAction()
        {
            _client.InteractionCreated += HandleInteraction;

            _client.SlashCommandExecuted += client_SlashCommandExecuted;
            _client.ButtonExecuted += client_ButtonExecuted;
            _client.SelectMenuExecuted += _client_SelectMenuExecuted;
            _client.ModalSubmitted += _client_ModalSubmitted;
        }

        private async Task _client_ModalSubmitted(SocketModal arg)
        {
            string id = arg.Data.CustomId;

            var info = id.Split('_');

            try
            {
                switch (info[0])
                {
                    case "main":
                        break;
                    //방주
                    case "mrfz":
                        //await ToolBoxMain.ReadCommand(arg, info[1]);
                        break;
                    //마작
                    case "mahj":
                        await MahjongMain.ModalSubmitted(arg, info[1]);
                        break;

                }
                await arg.RespondAsync(arg.Data.CustomId);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine($"ErrorOccured while Execute {arg.Data.CustomId} ModalSubmit in {arg.GuildId}, {arg.ChannelId}");
                Console.WriteLine(e.Message);
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(Config.commandDivisionLine);
                Console.ResetColor();
            }
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch
            {

            }
        }

        private async Task _client_SelectMenuExecuted(SocketMessageComponent arg)
        {
            string id = arg.Data.CustomId;

            var info = id.Split('_');

            //SocketInteraction a = arg;
            try
            {
                switch (info[0])
                {
                    case "main":
                        break;
                    //방주
                    case "mrfz":
                        //await ToolBoxMain.ReadCommand(arg, info[1]);
                        break;
                    //마작
                    case "mahj":
                        await MahjongMain.SelectMenuExecute(arg, info[1]);
                        break;

                }
                await arg.RespondAsync(arg.Data.CustomId);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine($"ErrorOccured while Execute {id} SlashCommand in {arg.GuildId}, {arg.ChannelId}");
                Console.WriteLine(e.Message);
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(Config.commandDivisionLine);
                Console.ResetColor();
            }

            //switch (arg.Data.CustomId)
            //{
            //    case "mahj_player":
            //    case "mahj_style":

            //        //var value = arg.Data.Values.First();
            //        //var menu = new SelectMenuBuilder()
            //        //{
            //        //    CustomId = arg.Data.CustomId,
            //        //    Placeholder = $"{(arg.Message.Components.First().Components.First() as SelectMenuComponent).Options.FirstOrDefault(x => x.Value == value).Label}",
            //        //    MaxValues = 1,
            //        //    MinValues = 1
            //        //};

            //        //await arg.UpdateAsync(x =>
            //        //{
            //        //    x.Components = arg.Message.;
            //        //});

            //        await arg.DeferAsync();
            //        break;
            //}
        }

        private async Task client_ButtonExecuted(SocketMessageComponent arg)
        {
            string id = arg.Data.CustomId;

            var info = id.Split('_');

            //SocketInteraction a = arg;
            try
            {
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

            //await arg.RespondAsync(arg.Data.CustomId);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine($"ErrorOccured while Execute {id} SlashCommand in {arg.GuildId}, {arg.ChannelId}");
                Console.WriteLine(e.Message);
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(Config.commandDivisionLine);
                Console.ResetColor();
            }
        }

        private async Task client_SlashCommandExecuted(SocketSlashCommand command)
        {
            string name = command.CommandName;

            var commandData= name.Split('_');

            //switch(commandData[0])
            try
            {
                switch(name)
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
                    case "startgame":
                        await MahjongMain.ReadCommand(command, name);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine($"ErrorOccured while Execute {name} SlashCommand in {command.GuildId}, {command.ChannelId}");
                Console.WriteLine(e.Message);
                Console.WriteLine(Config.commandDivisionLine);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(Config.commandDivisionLine); 
                Console.ResetColor();
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
                .WithButton("마작", "mahj_main");
            

            await command.RespondAsync(embed:  embed.Build() , components: builder.Build());
        }

        
    }
}
