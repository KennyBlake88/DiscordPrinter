using System;
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
        public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        public static DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = "ODI3NTA0ODE5ODAzMzg5OTYy.YGb_6w.yQjMRFs7_ROd4qewB1LCck9tP6E";

            _client.Log += _client_Log;
            _client.MessageReceived += MessageReceiver.HandleMessageReceived;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, token);
            await Printer.initialize();
            await _client.StartAsync();
                
            

            await Task.Delay(-1);
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
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot)
            {
                return;   
            }

            int argPos = 0;
            if (message.HasStringPrefix("&", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        } 


    }
}
