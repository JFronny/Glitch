using System;
using System.Diagnostics;
using System.Drawing;
using ScreenLib;

namespace DXOS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            long total = 0;
            sw.Start();
            Image img = ScreenMan.CaptureScreen();
            sw.Stop();
            total += sw.ElapsedMilliseconds;
            Console.WriteLine($"Capture Screen: {sw.ElapsedMilliseconds.ToString("G")}ms");

            sw.Restart();
            Graphics g = Graphics.FromImage(img);
            sw.Stop();
            total += sw.ElapsedMilliseconds;
            Console.WriteLine($"Open graphics object: {sw.ElapsedMilliseconds.ToString("G")}ms");

            sw.Restart();
            g.Clear(Color.White);
            g.DrawRectangle(Pens.Red, ScreenMan.GetBounds());
            g.DrawLine(Pens.Red, 0, 0, img.Width, img.Height);
            g.DrawLine(Pens.Red, 0, img.Height, img.Width, 0);
            sw.Stop();
            total += sw.ElapsedMilliseconds;
            Console.WriteLine($"Drawing test: {sw.ElapsedMilliseconds.ToString("G")}ms");

            sw.Restart();
            ScreenMan.Draw(img);
            sw.Stop();
            total += sw.ElapsedMilliseconds;
            Console.WriteLine($"Write screen: {sw.ElapsedMilliseconds.ToString("G")}ms");

            Console.WriteLine(
                $"All tests ran. Total elapsed time (Except init steps): {total}ms.\r\nThis equals {1000d / total} fps.");
        }
    }
}