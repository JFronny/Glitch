using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using ImageProcessor;
using ImageProcessor.Imaging;
using ScreenLib;

namespace AForge_Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Color[] colors = Enum.GetValues(typeof(KnownColor)).OfType<KnownColor>()
                .Select(s => Color.FromKnownColor(s)).ToArray();
            Random rnd = new Random();
            while (true)
            {
                Thread.Sleep(70);
                using MemoryStream ms = new MemoryStream();
                using ImageFactory imageFactory = new ImageFactory();
                imageFactory.Load(ScreenMan.CaptureScreen())
                    .Pixelate(4)
                    .Watermark(new TextLayer
                    {
                        Text = "CC24 PiXl",
                        Position = Point.Empty,
                        FontColor = colors[rnd.Next(colors.Length)]
                    })
                    .Save(ms);
                ms.Position = 0;
                ScreenMan.Draw(Image.FromStream(ms));
            }
        }
    }
}