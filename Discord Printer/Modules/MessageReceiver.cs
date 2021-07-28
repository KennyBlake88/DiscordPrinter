using System;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Drawing;


namespace Discord_Printer.Modules
{
    public abstract class MessageReceiver
    {
        private static readonly Emoji checkMark = new Emoji("✅");
        private static readonly Emoji xMark = new Emoji("❌");
        private static readonly ulong mainId = 829189785528172544;
        private static readonly ulong testId = 827543130017759244;
        private static bool active = false;
        public static Task HandleMessageReceived(IMessage message)
        {
            if (!message.Author.IsBot)
            {
                if (active)
                {
                    if (message.Channel.Id.Equals(mainId) || message.Channel.Id.Equals(testId))
                    {
                        if (message.Attachments.Count <= 1)
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
                                    var i = new Picture(att);
                                    if (i.validated)
                                    {
                                        if (i.isAdult || i.isRacy)
                                        {
                                            message.DeleteAsync();
                                            message.Channel.SendMessageAsync($"{message.Author.Mention}, I detected that as adult content. I've deleted the message, and this has been logged.");
                                            LogMessage(i, message.Author.Id, message.Author.Username, i.filePath, i.fileName, true);

                                        }
                                        else if (i.isGory)
                                        {
                                            message.DeleteAsync();
                                            message.Channel.SendMessageAsync($"{message.Author.Mention}, I detected that as gory content. I've deleted the message, and this has been logged.");
                                            LogMessage(i, message.Author.Id, message.Author.Username, i.filePath, i.fileName, true);
                                        }
                                        else
                                        {
                                            BestImageGetter.addToThisSet(message, i);
                                            message.AddReactionAsync(checkMark);
                                        }
                                        LogMessage(i, message.Author.Id, message.Author.Username, i.filePath, i.fileName, false);
                                    }
                                    else
                                    {
                                        message.AddReactionAsync(xMark);
                                        message.Channel.SendMessageAsync($"{message.Author.Mention}, We both know thats not a real image file my guy.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            message.Channel.SendMessageAsync($"{message.Author.Mention}, you can only send 1 image at a time!");
                            message.AddReactionAsync(xMark);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        public static Task acitvate(bool a)
        {
            active = a;
            return Task.CompletedTask;
        }

        private static void LogMessage(Picture i, ulong id, string username, string path, string fileName, bool flagged)
        {
            string firstText = $"{id}, {username}";

            PointF firstLocation = new PointF(10f, 10f);
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);//load the image file

            using ( Graphics graphics = Graphics.FromImage(image))
            {
                using (Font arialFont = new Font("Arial", 10))
                {
                    graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, 300, 50));
                    graphics.DrawString(firstText, arialFont, Brushes.Black, firstLocation);
                }
            }
            if (flagged)
            {
                image.Save($"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\logs\\FLAGGED{id.ToString()}-{fileName}");//save the image file'
            }
            else
            {
                image.Save($"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\logs\\{id.ToString()}-{fileName}");//save the image file'
            }
        }

    }
}
