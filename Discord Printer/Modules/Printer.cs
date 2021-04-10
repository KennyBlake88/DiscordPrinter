using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace Discord_Printer.Modules
{
    public abstract class Printer
    {
        private static System.ComponentModel.Container components;
        private static StreamReader streamToPrint;
        private static PrintDocument pd;
  
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

        public static async Task printImage(Picture i)
        {
            streamToPrint = new StreamReader(i.getFile());
            try
            { 
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (thesender, ev) => {
                    ev.Graphics.DrawImage(Image.FromFile(i.getFile()),
                    //This is to keep image in margins of the Page.
                    new PointF(ev.MarginBounds.Left, ev.MarginBounds.Top));
                };
                pd.Print();

            } catch (InvalidPrinterException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
