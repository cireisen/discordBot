using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Interactions;
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
using System.Windows.Input;

namespace discordBot
{
    internal class SuperHandler
    {
        private readonly DiscordSocketClient _client;
        private InteractionService _command;
        private IServiceProvider _services;
        private DiscordBotListHandler _discordBotListHandler;


        public SuperHandler(DiscordSocketClient client) 
        {
            _client = client;
            _discordBotListHandler = new DiscordBotListHandler(1075936880853520434, Config.Token, _client);
        }

        public async Task StartBotAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandManager>()
                .BuildServiceProvider();

            _command = _services.GetRequiredService<InteractionService>();
            _command.Log += Log;

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
            await _services.GetService<CommandManager>().InitializeCommand();
            await _command.RegisterCommandsGloballyAsync(true);
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


            if (s.Author.IsBot)
            {
                return;
            }
            
            Console.WriteLine(msg.Content);
            Console.WriteLine(msg.Author.ToString());

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
    }
}
