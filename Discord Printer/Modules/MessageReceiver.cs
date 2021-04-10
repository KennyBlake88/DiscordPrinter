using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;

namespace Discord_Printer.Modules
{
    public abstract class MessageReceiver
    {
        private static readonly Emoji checkMark = new Emoji("✔");
        private static readonly Emoji xMark = new Emoji("❌");
        private static readonly ulong mainId = 829189785528172544;
        private static readonly ulong testId = 827543130017759244;
        public static Task HandleMessageReceived(IMessage message)
        {
            if (!message.Author.IsBot)
            {
                if (message.Channel.Id.Equals(mainId) || message.Channel.Id.Equals(testId))
                {
                    if (message.Attachments.Count > 0)
                    {
                        foreach (IAttachment att in message.Attachments)
                        {
                            if (!(att.Filename.EndsWith("jpg") || att.Filename.EndsWith("png")))
                            {
                                message.Channel.SendMessageAsync($"{message.Author.Mention}, I can't print that filetype! I can only print .png or .jpg!");
                                message.AddReactionAsync(xMark);
                            }
                            else
                            {
                                message.AddReactionAsync(checkMark);
                                var i = new Picture(att);
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

    }
}
