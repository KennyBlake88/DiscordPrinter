using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;


namespace Discord_Printer.Modules
{
    public class Picture
    {
        public static string mostRecentFile = "";
        public string fileName { get; private set; }
        public bool validated { get; private set; }
        private static ComputerVisionClient cV;
        private static int counter = 0;
        public string filePath { get; private set; }

        List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>() 
        { VisualFeatureTypes.Description, VisualFeatureTypes.Adult };

        public string description { get; private set; }
        public bool isAdult { get; private set; }
        public bool isGory { get; private set; }
        public bool isRacy { get; private set; }


        public Picture(IAttachment att)
        {
            validated = false;
            this.fileName = att.Filename;
            using (var client = new WebClient())
            {
                if (ImageValidator.validateImage(att))
                {
                    validated = true;
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            filePath = $"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Images\\{att.Filename}";
                            if (File.Exists(filePath))
                            {
                                counter++;
                                string name = att.Filename.Substring(0, att.Filename.IndexOf("."));
                                string extension = att.Filename.Substring(att.Filename.IndexOf("."), (att.Filename.Length - name.Length));
                                name = $"{name}{counter}{extension}";
                                filePath = $"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Images\\{name}";
                                Console.WriteLine(filePath);
                            }
                            else
                            {
                                filePath = $"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Images\\{att.Filename}";
                            }
                            try
                            {
                                webClient.DownloadFile(att.Url, filePath);
                            }
                            catch (WebException ex)
                            {
                                Console.WriteLine("The same file tried being used twice.");
                            }
                        }

                        var t1 = processImage(filePath);
                        try
                        {
                            Task.WhenAll(t1).Wait();

                        }
                        catch (System.AggregateException ex)
                        {
                            Console.WriteLine("Bad Request! Breaking.");
                            isAdult = true; //if the image can't be analyzed, we're going to assume we can't print it for safety. 
                        }
                    } catch (IOException ex)
                    {
                        Console.WriteLine("Couldn't download file.");
                    }
                }

            }
        }

        private async Task processImage(string imagePath)
        {
            Console.WriteLine($"Processing {imagePath}");
            if (!File.Exists(imagePath))
            {
                Console.WriteLine("\nUnable to open or read localImagePath:\n{0} \n", imagePath);
                description = "File doesn't exist. Kenny fucked the code up lmao what an idiot";
            }
            else
            {
                using (Stream imageStream = File.OpenRead(imagePath))
                {
                    ImageAnalysis analysis = await cV.AnalyzeImageInStreamAsync(imageStream, features);
                    Console.WriteLine("finished analyzing");
                    this.isAdult = analysis.Adult.IsAdultContent;
                    this.isGory = analysis.Adult.IsGoryContent;
                    this.isRacy = analysis.Adult.IsRacyContent;
                    description = analysis.Description.Captions[0].Text;
                    Console.WriteLine($"{description}, adult:{isAdult}, gory:{isGory}, racy:{isRacy}");
                    imageStream.Close();
                }
            }
        }

        public static async Task initializeAzure(string key)
        {
            cV = Authenticate("https://discordprinter.cognitiveservices.azure.com/", key);
            Console.WriteLine(cV.Credentials.ToString());
        }

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }
    }
}
