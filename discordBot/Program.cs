using Discord;
using Discord.WebSocket;
using System;

namespace discordBot // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        private static DiscordSocketClient _client;
        private SuperHandler _handler;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            new Program().StartAsync().GetAwaiter().GetResult();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(e.ToString());
            Console.ResetColor();

            Console.WriteLine(e.ToString());
        }

        private async Task StartAsync()
        {
            Config.StartConfig();
            string botToken = Config.Token;

            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("No Token detected.");
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = false
            });

            _handler = new SuperHandler(_client);
            await _handler.StartBotAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await _client.SetGameAsync("testing", null, ActivityType.Listening);
            await ConsoleRead();
            await Task.Delay(-1);
        }

        private async Task ConsoleRead()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                if (input.ToLower() == "announce")
                {
                    Console.WriteLine("What do you want to send?");
                    var what = Console.ReadLine();
                    foreach (var clientGuild in _client.Guilds)
                    {
                        try
                        {
                            await clientGuild.DefaultChannel.SendMessageAsync(what);
                            Console.WriteLine(
                                $"Sent to {clientGuild.Name}@{clientGuild.DefaultChannel.Name} successfully.");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Could not send to {clientGuild.Name}.");
                        }
                    }
                }
                else if (input.ToLower() == "dm")
                {
                    for (var i = 0; i < _client.Guilds.Count; i++)
                    {
                        Console.WriteLine(i + 1 + _client.Guilds.ElementAt(i).Name);
                    }

                    var to = int.Parse(Console.ReadLine());
                    var guild = _client.Guilds.ElementAt(to - 1);
                    Console.WriteLine($"{guild.Name}'s channel ID please: ");
                    var id = ulong.Parse(Console.ReadLine());
                    var chanel = guild.GetTextChannel(id);
                    Console.WriteLine("What do u want to send");
                    var text = Console.ReadLine();
                    await chanel.SendMessageAsync(text);
                }

                await Task.CompletedTask;
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}
