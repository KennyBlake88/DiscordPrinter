using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord_Printer.Modules;

namespace Discord_Printer
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")]
        public async Task Ping()
        {
            await ReplyAsync($"I'm connected! Latency is: {Program._client.Latency}ms");
        }

        [Command("Start")]
        public async Task start()
        {
            await ReplyAsync("If I was smart this would do something but I'm just testing it it's working");
        }

        [Command("YM")]
        public async Task ym()
        {
            await ReplyAsync("Ya Mama");
        }

        [Command("Describe")]
        public async Task describe()
        {
            //the code to do the stuff will appear here.
        }
    }
}
