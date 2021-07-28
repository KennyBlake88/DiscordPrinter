using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord_Printer.Modules;
using Microsoft.Extensions.DependencyInjection;


namespace Discord_Printer
{
    class Program
    {
        private static string t1;
        private static string t2;
        private static DiscordSocketConfig config;
        public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            await initialize();
            _client = new DiscordSocketClient(config);
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();



            _client.Log += _client_Log;
            _client.MessageReceived += MessageReceiver.HandleMessageReceived;
            _client.ReactionAdded += BestImageGetter.addMessageReactionCount;
            _client.ReactionRemoved += BestImageGetter.subMessageReactionCount;

            await Picture.initializeAzure(t2);
            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, t1);
            await Printer.initialize();
            await _client.StartAsync();



            await Task.Delay(-1);
        }

        private Task initialize()
        {
            config = new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100
            };

            using (StreamReader inputFile = new StreamReader("C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Modules\\secrets.txt"))
            {
                t1 = inputFile.ReadLine();
                t2 = inputFile.ReadLine();
                Console.WriteLine($"t1: {t1} t2: {t2}");
            }
            return Task.CompletedTask;
        }

        private Task _client_Log(LogMessage arg)
        {
            //throw new NotImplementedException();
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is SocketUserMessage)
            {
                var message = arg as SocketUserMessage;
                var context = new SocketCommandContext(_client, message);
                if (message.Author.IsBot)
                {
                    return;
                }

                int argPos = 0;
                if (message.HasStringPrefix("=", ref argPos))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                }
            }
          
        }
       
    }
}
