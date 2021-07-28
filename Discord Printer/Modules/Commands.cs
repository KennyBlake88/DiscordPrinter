using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord_Printer.Modules;
using Discord.Addons.Interactive;
using System.IO;

namespace Discord_Printer
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private InteractiveService inter = new InteractiveService(Program._client);
        [Command("Ping")]
        public async Task Ping()
        {
            await ReplyAsync($"I'm connected! Latency is: {Program._client.Latency}ms");
        }

        [Command("Start")]
        public async Task start()
        {
            ulong id = 321139129419300864;
            if (Context.User.Id != id)
            {
                await ReplyAsync("Sorry, only kenny can execute commands cause hes a lazy programmer.");
            }
            else
            {
                //await ReplyAsync("going");
                await ReplyAsync("Have fun printing! Any Complaints can be sent to <@396113709170425858>.");
                BestImageGetter.initialize(Context.Message);
                await MessageReceiver.acitvate(true);
            }
    }

        [Command("Stop")]
        public async Task stop()
        {
            ulong id = 321139129419300864;
            if (Context.User.Id != id)
            {
                await ReplyAsync("Sorry, only kenny can execute commands cause hes a lazy programmer.");
            }
            else
            {
                await ReplyAsync("Bot is down while I get more ink/fix something/get more paper. Any complaints can be sent to <@396113709170425858>");
                BestImageGetter.stop();
                await MessageReceiver.acitvate(false);
            }
        }

        [Command("YM")]
        public async Task ym()
        {
            await ReplyAsync("Ya Mama");
        }

        [Command("Purge")]
        public async Task purge()
        {
            ulong id = 321139129419300864;
            if (Context.User.Id != id)
            {
                await ReplyAsync("Sorry, only kenny can execute commands cause hes a lazy programmer.");
            }
            else
            {
                bool deleted = true;
                System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Images\");
                int amnt = di.GetFiles().Length;
                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        deleted = true;
                        file.Delete();
                    }
                    catch (IOException ex)
                    {
                        amnt--;
                        deleted = false;
                        Console.WriteLine(ex.Message);
                        await ReplyAsync($"Couldn't delete file {file.Name}. It is currently being used by the printing process.");
                    }
                }
                if (deleted)
                {
                    await ReplyAsync($"Deleted {amnt} files.");
                }
            }
        }
        [Command("apply_roles")]
        public async Task apply_roles()
        {
            IRole role = Program._client.GetGuild(756025919063326752ul).GetRole(756026419590725633ul);
            
            await foreach(IGuildUser user in Program._client.GetGuild(756025919063326752ul).GetUsersAsync())
            {
                if (!(user.RoleIds.Contains(756025919063326752ul)))
                {
                   await user.AddRoleAsync(role);
                }
            }
        }

        /*
        [Command("Describe")]
        public async Task describe()
        {
            await ReplyAsync($"There are {Picture.describedImages.Count} images that have been processed.");
            string replyString = "";
            for (int i = 0; i < Picture.describedImages.Count; i++)
            {
                replyString += ($"{i}. {Picture.describedImages[i]}");
            }

            await ReplyAsync(replyString);
            await ReplyAsync("Choose which image to describe now:");

            var response = await inter.NextMessageAsync();
            if (response != null)
                await ReplyAsync($"You replied: {response.Content}");
            else
                await ReplyAsync("You did not reply before the timeout");
            }
        }*/
    }
}
