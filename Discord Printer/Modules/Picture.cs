using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.IO;

namespace Discord_Printer.Modules
{

    public class Picture
    {
        private string filePath;
        public Picture(IAttachment att)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(att.Url, $"C:/Users/kenny/source/repos/Discord Printer/Discord Printer/Images/{att.Filename}");
                this.filePath = $"C:/Users/kenny/source/repos/Discord Printer/Discord Printer/Images/{att.Filename}";

                _ = Printer.printImage(this);
            }

        }
        public string getFile()
        {
            return this.filePath;
        }
    }
}
