using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace discordBot
{
    internal class SuperHandler
    {
        private readonly DiscordSocketClient _client;
        private CommandService _command;
        private IServiceProvider _services;
        private DiscordBotListHandler _discordBotListHandler;


        public SuperHandler(DiscordSocketClient client) 
        {
            _client = client;
            _discordBotListHandler = new DiscordBotListHandler(1075936880853520434, Config.Token, _client);
        }

        public async Task StartBotAsync()
        {
            _command = new CommandService(new CommandServiceConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_command)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();

            await _command.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.Ready += Ready;
            _client.MessageReceived += HandleCommandAsync;
            _client.JoinedGuild += JoinAsync;
            _client.LeftGuild += LeftAsync;
            _client.Log += Log;
        }
        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            CommandManager manager = new CommandManager(_client);
            manager.CreateSlashCommands();
            manager.SetCommandAction();
            await _discordBotListHandler.UpdateAsync();
        }

        private async Task LeftAsync(SocketGuild arg)
        {
            await _discordBotListHandler.UpdateAsync();
            var channel = _client.GetGuild(Config.ControlGuildID).GetChannel(Config.ControlChannelID) as ITextChannel;
            if (channel is null) return;
            await channel.SendMessageAsync($"I just left {arg.Name} :slight_frown: ");
        }

        private async Task JoinAsync(SocketGuild arg)
        {
            await _discordBotListHandler.UpdateAsync();
            if (!(_client.GetGuild(Config.ControlGuildID).GetChannel(Config.ControlChannelID) is ITextChannel channel))
                return;
            await channel.SendMessageAsync($"I just joined {arg.Name} :tada:");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
                
            if (!(s is SocketUserMessage msg))
            {
                return;
            }

            var context = new SocketCommandContext(_client, msg);
            var argPos = 0;

            #region Dota Update

            //var embed = s.Embeds.FirstOrDefault();
            //if (s.Channel.Id == 437635625567649804 && s.Author.IsWebhook)
            //{
            //    if (embed?.Author?.Name.StartsWith("Dota") == true)
            //    {
            //        await BroadcastUpdate(embed);
            //    }
            //}

            #endregion

            if (s.Author.IsBot)
            {
                return;
            }
            
            Console.WriteLine(msg.Content);
            Console.WriteLine(msg.Author.ToString());
            //#region Prefix Management
            //if (!Config.Bot.PrefixDictionary.ContainsKey(context.Guild.Id))
            //{
            //    Config.Bot.PrefixDictionary.Add(context.Guild.Id, "$");
            //    Config.Saave();
            //}
            //#endregion

            #region Command Management
            if (msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                using (context.Channel.EnterTypingState())
                {

                    try
                    {
                        var result = await _command.ExecuteAsync(context, argPos, _services);
                        if (!result.IsSuccess)
                        {
                            Console.WriteLine(result.ErrorReason + $" at {context.Guild.Name}");    
                            switch (result.Error)
                            {
                                case CommandError.UnknownCommand:
                                    {
                                        var guildEmote = Emote.Parse("<:unknowscmd:461157571701506049>");
                                        await msg.AddReactionAsync(guildEmote);
                                        break;
                                    }
                                case CommandError.BadArgCount:
                                    {
                                        await context.Channel.SendMessageAsync(
                                            "You are suppose to pass in a parameter with this" +
                                            " command. type `help [command name]` for help");
                                        break;
                                    }
                                case CommandError.UnmetPrecondition:
                                    {
                                        await context.Channel.SendMessageAsync(
                                            "You can not use this command at the moment.\nReason: " +
                                            result.ErrorReason);
                                        break;
                                    }
                                default:
                                    {
                                        await context.Channel.SendMessageAsync(result.Error.ToString());
                                        break;
                                    }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        Console.ResetColor();
                    }
                }
            }
            
            #endregion

            if(msg.Content.Contains("과누"))
            {
                try
                {
                    EmbedBuilder image = new EmbedBuilder();
                    //string uri = @"https://cdn.discordapp.com/attachments/931796153081683992/1082935842798063656/U25J6mOepQTzY-V4Rnx4WkeKqIHTKMB4DaAW3i8Ct_ZC3UyFmNKaf-mKEqGTKLJNor_JLCLVxF57qSdkK56Nn1CDLvmG_mvWpH-N3UKWiAhjPkCzNq9alhcEBQJKQBx67821igVR2qCIXosniIC7vA.png";
                    //image.WithImageUrl(uri);

                    string path = @"C:\Users\KDH\source\repos\discordBot\discordBot\bin\Debug\net6.0\resource\images.jpg";
                    var filename = Path.GetFileName(path);

                    image.WithImageUrl($"attachment://{filename}");
                        
                    await msg.Channel.SendFileAsync(path, $"<@{msg.Author.Id}>아 날속인거니?", false, image.Build());
                    Console.WriteLine(
                        $"Sent to {msg.Channel} successfully.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Could not send to {msg.Channel}.");
                }
            }
            else if(msg.Content.Contains("Sat0ru"))
            {
                try
                {

                    await msg.Channel.SendMessageAsync($"去当兵吧");
                    Console.WriteLine(
                        $"Sent to {msg.Channel} successfully.");
                }
                catch (Exception)
                {
                    Console.WriteLine($"Could not send to {msg.Channel}.");
                }
            }
            else if(msg.Author.Username == "cireisen")
            {
                
            }
        }

        private async Task BroadcastUpdate(Embed embed)
        {
            //foreach (var data in UpdateReceivers.Patches)
            //{
            //    await _client.GetGuild(data.GuildId).GetTextChannel(data.ChannelId).SendMessageAsync(embed: embed);
            //}
        }

        private async Task AddSlashCommand()
        {
            
        }
    }
}
