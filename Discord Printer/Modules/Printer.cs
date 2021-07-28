using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Discord_Printer.Modules
{
    public abstract class Printer
    {
  
        //private static PrintServer printServer;
        public static Task initialize()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
             
                if (printer.Contains("HP"))
                {
                   Console.WriteLine($"Printer Found: " + printer);
                }
            }
            return Task.CompletedTask;
        }

        public static async Task printImage(string fn)
        {
            using (var streamToPrint = new StreamReader(fn))
            {
                try
                {
                    PrintDocument pd = new PrintDocument();
                    pd.DefaultPageSettings.Landscape = true;
                    pd.PrintPage += (thesender, ev) =>
                    {
                        ev.Graphics.DrawImage(Image.FromFile(fn),
                        //This is to keep image in margins of the Page.
                        ev.MarginBounds);
                    };
                    pd.Print();
                }
                catch (InvalidPrinterException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
