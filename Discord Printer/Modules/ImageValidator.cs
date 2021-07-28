using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.AccessControl;

namespace Discord_Printer.Modules
{
    public abstract class ImageValidator
    {
        public static bool validateImage(IAttachment att)
        {
            byte[] fileBytes;
            bool validated = false;
            WebClient webClient = new WebClient();

            string filePath = $"C:\\Users\\kenny\\source\\repos\\Discord Printer\\Discord Printer\\Images\\{att.Filename}";
            
            fileBytes = webClient.DownloadData(att.Url);

            var headers = new List<byte[]>
            {
                new byte[] { 137, 80, 78, 71 },     // PNG
                new byte[] { 255, 216, 255},  // JPEG
            };
            if (headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length))))
            {
                validated = true;
            }

            return validated;
        }
    }
}
