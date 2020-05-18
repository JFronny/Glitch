using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using CC_Functions.W32;
using CC_Functions.W32.DCDrawer;
using CC_Functions.W32.Forms;
using GlitchPayloads.Properties;
using Misc;
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace GlitchPayloads
{
    [PayloadClass]
    public static class Broadcast
    {
        [Payload("Move windows", false, 50, 2000)]
        public static void PayloadWindowMove()
        {
            Wnd32 tmp = Wnd32.Foreground;
            Rectangle pos = tmp.Position;
            pos.X += Common.Rnd.Next(-2, 3);
            pos.Y += Common.Rnd.Next(-2, 3);
            tmp.Position = pos;
        }

        [Payload("Draw text to desktop", false, 200, 4000)]
        public static void PayloadDesktopText()
        {
            using IDCDrawer drawer = DeskMan.CreateGraphics();
            drawer.Graphics.DrawString("We are watching.",
                new Font(FontFamily.Families[Common.Rnd.Next(FontFamily.Families.Length)], 15),
                new SolidBrush(Color.FromArgb(Common.Rnd.Next(256), Common.Rnd.Next(256), Common.Rnd.Next(256))),
                Common.Rnd.Next(Screen.PrimaryScreen.WorkingArea.Width),
                Common.Rnd.Next(Screen.PrimaryScreen.WorkingArea.Height));
        }

        [Payload("Randomly change time", false, 300, 4000)]
        public static void PayloadTime()
        {
            DateTime time = new DateTime(1995, 1, 1);
            double range = (new DateTime(9999, 12, 31) - time).TotalDays;
            time = time.AddDays(Common.Rnd.NextDouble() * range)
                .AddHours(Common.Rnd.Next(24))
                .AddMinutes(Common.Rnd.Next(60))
                .AddSeconds(Common.Rnd.Next(60))
                .AddMilliseconds(Common.Rnd.Next(1000));
            Time.Set(time);
        }

        [Payload("Draw Jumpscares", true, 300, 16000)]
        public static void PayloadJumpscare()
        {
            using (IDCDrawer drawer = ScreenMan.GetDrawer())
                drawer.Graphics.DrawImage(Resources.jumpscare, Screen.PrimaryScreen.Bounds);
            Console.Beep(2000, 800);
            using (IDCDrawer drawer = ScreenMan.GetDrawer()) drawer.Graphics.Clear(Color.Black);
        }

        [Payload("Eye on Desktop", false, 0, 0, true)]
        public static void PayloadDesktopEyes()
        {
            using IDCDrawer drawer = DeskMan.CreateGraphics();
            Pen eye = new Pen(new SolidBrush(Color.Red), 2);
            drawer.Graphics.DrawCurve(eye, new[] {MakePoint(20, 50), MakePoint(50, 65), MakePoint(80, 50)});
            drawer.Graphics.DrawCurve(eye, new[] {MakePoint(20, 50), MakePoint(50, 35), MakePoint(80, 50)});
            drawer.Graphics.DrawEllipse(eye,
                new RectangleF(
                    PointF.Subtract(
                        new PointF(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2),
                        MakeSizeY(15, 15)), MakeSizeY(30, 30)));
        }

        private static PointF MakePoint(float xPercent, float yPercent) => new PointF(
            Screen.PrimaryScreen.Bounds.Width * xPercent / 100,
            Screen.PrimaryScreen.Bounds.Height * yPercent / 100);

        private static SizeF MakeSizeY(float xPercent, float yPercent) => new SizeF(
            Screen.PrimaryScreen.Bounds.Height * xPercent / 100,
            Screen.PrimaryScreen.Bounds.Height * yPercent / 100);

        [Payload("Show cool popup", false, 0, 0, true)]
        public static void PayloadBroadcastPopup()
        {
            new THEEYESTHEEYES().ShowDialog();
        }

        public sealed class THEEYESTHEEYES : Form
        {
            private bool _pCl = true;
            private readonly Label label1;

            public THEEYESTHEEYES()
            {
                label1 = new Label();
                SuspendLayout();
                label1.AutoSize = true;
                label1.Font = new Font("Microsoft Sans Serif", 40F, FontStyle.Regular, GraphicsUnit.Point, 0);
                label1.ForeColor = Color.White;
                label1.Location = new Point(0, 0);
                label1.Name = "label1";
                label1.Size = new Size(1017, 63);
                label1.TabIndex = 0;
                label1.Text = "BROADCAST WILL RESUME SHORTLY";
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                BackColor = Color.Blue;
                ClientSize = new Size(1276, 450);
                Controls.Add(label1);
                ForeColor = Color.White;
                FormBorderStyle = FormBorderStyle.None;
                Name = "THEEYESTHEEYES";
                WindowState = FormWindowState.Maximized;
                Load += THEEYESTHEEYES_Load;
                Closing += THEEYESTHEEYES_OnClosing;
                ResumeLayout(false);
                PerformLayout();
                TopMost = true;
                this.GetWnd32().Overlay = true;
            }

            private void THEEYESTHEEYES_Load(object sender, EventArgs e)
            {
                label1.Top = Height / 2 - label1.Height / 2 - 20;
                label1.Left = Width / 2 - label1.Width / 2;
                BackColor = Color.FromArgb(SystemParameters.WindowGlassColor.A, SystemParameters.WindowGlassColor.R,
                    SystemParameters.WindowGlassColor.G, SystemParameters.WindowGlassColor.B);
                RotatingIndicator p = new RotatingIndicator();
                Controls.Add(p);
                p.Size = new Size(50, 50);
                p.Left = Width / 2 - p.Width / 2;
                p.Top = Height - 250;
                new Thread(ThreadT).Start();
                Cursor.Hide();
            }

            private void ThreadT()
            {
                Thread.Sleep(3000);
                Invoke((MethodInvoker) (() =>
                {
                    Cursor.Show();
                    _pCl = false;
                    Close();
                }));
            }

            private void THEEYESTHEEYES_OnClosing(object sender, CancelEventArgs e) => e.Cancel = _pCl;
        }
    }
}