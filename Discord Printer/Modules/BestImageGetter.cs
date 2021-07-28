using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Discord_Printer.Modules
{
    abstract class BestImageGetter
    {
        private static IMessageChannel channel;
        private static Dictionary<ulong, int> messagesInThisSet = new Dictionary<ulong, int>();
        private static Dictionary<ulong, Picture> picturesInThisSet = new Dictionary<ulong, Picture>();
        private static Picture bestPicture;
        private static readonly Emoji printer = new Emoji("🖨");
        private static readonly Emoji checkMark = new Emoji("✅");
        private static ulong bestMessageID;
        private static bool running = false;
        private static Picture previousPicture;

        public static void addToThisSet(IMessage message, Picture p)
        {
            messagesInThisSet.Add(message.Id, 0);
            picturesInThisSet.Add(message.Id, p);
        }

        public static async Task addMessageReactionCount(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            if(originChannel.Id == channel.Id)
            {
                if(messagesInThisSet.ContainsKey(cachedMessage.Id))
                {
                    if (reaction.Emote.Name == checkMark.Name)
                    {
                        messagesInThisSet[cachedMessage.Id]++;
                    }
                }
            }
        }

        public static async Task subMessageReactionCount(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            if (originChannel.Id == channel.Id)
            {
                if (messagesInThisSet.ContainsKey(cachedMessage.Id))
                {
                    if (reaction.Emote.Name == checkMark.Name)
                    {
                        messagesInThisSet[cachedMessage.Id]--;
                    }
                }
            }
        }

        private static ulong getBestMessage()
        {
            ulong bestMessageID= 0;
            int maximum = 0;

            foreach (ulong id in messagesInThisSet.Keys)
            {
                if (messagesInThisSet[id] > maximum)
                {
                    maximum = messagesInThisSet[id];
                    bestMessageID = id;
                }
            }
            return bestMessageID;

        }

        public static async Task stop()
        {
            running = false;
        }

        public static async Task initialize(IMessage message)
        {
            channel = message.Channel;
            running = true;
            await run();
        }

        private static async Task run()
        {
            int counter = 0;
            while (running)
            {
                switch (counter)
                {
                    case 0:
                        await channel.SendMessageAsync("10 Minutes until the next image is chosen! Start sending/voting!");
                        counter++;
                        break;

                    case 60:
                        channel.SendMessageAsync("9 Minutes left!");
                        counter++;
                        break;

                    case 180:
                        channel.SendMessageAsync("7 Minutes left!");
                        counter++;
                        break;

                    case 300:
                        await channel.SendMessageAsync("5 minutes left!");
                        counter++;
                        break;

                    case 540:
                        await channel.SendMessageAsync("1 Minute left!");
                        counter++;
                        break;

                    case 570:
                        await channel.SendMessageAsync("30 Seconds Left!");
                        counter++;
                        break;

                    case 590:
                        await channel.SendMessageAsync("10 Seconds to go! Put your final votes in!");
                        counter++;
                        break;

                    case 600:
                        if (messagesInThisSet.Keys.Count > 0)
                        {
                            bestMessageID = getBestMessage();
                            bestPicture = picturesInThisSet[bestMessageID];
                            if (bestPicture != null)
                            {
                                if (bestPicture != previousPicture)
                                {
                                    await channel.SendMessageAsync($"{channel.GetMessageAsync(bestMessageID).Result.Author.Mention}, your image: {bestPicture.fileName} will now be printed!");
                                    await channel.GetMessageAsync(bestMessageID).Result.AddReactionAsync(printer);
                                    previousPicture = bestPicture;
                                    await Printer.printImage(bestPicture.filePath);
                                }
                                else
                                {
                                    channel.SendMessageAsync("The best picture didn't change, so we're just not gonna print it.");
                                }
                            }
                            else                           
                            {
                                channel.SendMessageAsync("There were no images, resetting!");
                            }
                        }
                        else
                        {
                            channel.SendMessageAsync("There were no images, resetting!");
                        }
                        messagesInThisSet = new Dictionary<ulong, int>();
                        picturesInThisSet = new Dictionary<ulong, Picture>();
                        bestMessageID = 0;
                        bestPicture = null;
                        counter = 0;
                        break;

                    default:
                    counter++;
                    break;
                }
                await Task.Delay(1000);
            }
        }
    }
}
