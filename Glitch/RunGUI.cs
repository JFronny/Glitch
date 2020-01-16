using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using CC_Functions.W32.Hooks;
using GlitchPayloads;

namespace Glitch
{
    public partial class RunGUI : Form
    {
        private static bool _payloadsEnabled = true;

        public RunGUI()
        {
            InitializeComponent();
            Program.payloads.ForEach(s =>
            {
                Button btn = new Button
                {
                    Text = s.Item1.GetPayloadName(),
                    Width = 200,
                    Height = 30,
                    BackColor = Color.FromKnownColor(KnownColor.Control),
                    Tag = new object[] {s, GetFRunner(s.Item1, s.Item2), false}
                };
                btnPanel.Controls.Add(btn);
                btn.Click += PayloadButtonClick;
            });
            new KeyboardHook().OnKeyPress += e =>
            {
                if (e.Key == Keys.Escape)
                    if (KeyboardReader.IsKeyDown(Keys.LShiftKey))
                    {
                        _payloadsEnabled = !_payloadsEnabled;
                        Text =
                            $"CC24 Glitch - Payloads are {(_payloadsEnabled ? "enabled" : "disabled")}. " +
                            "Press LShift+Esc to toggle";
                    }
            };
        }

        private void PayloadButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            object[] args = (object[]) btn.Tag;
            Tuple<MethodInfo, PayloadAttribute> s = (Tuple<MethodInfo, PayloadAttribute>) args[0];
            Thread th = (Thread) args[1];
            if (th.IsAlive)
            {
                th.Abort();
                args[1] = GetFRunner(s.Item1, s.Item2);
            }
            if ((bool) args[2])
            {
                args[2] = false;
                btn.BackColor = Color.FromKnownColor(KnownColor.Control);
                ScreenMan.Refresh();
            }
            else
            {
                th.Start();
                args[2] = true;
                btn.BackColor = Color.FromKnownColor(KnownColor.Highlight);
            }
            btn.Tag = args;
        }

        private Thread GetFRunner(MethodInfo method, PayloadAttribute data)
        {
            return new Thread(() =>
            {
                while (true)
                {
                    if (_payloadsEnabled)
                        method.Invoke(null, new object[0]);
                    if (IsDisposed)
                        return;
                    Thread.Sleep(Math.Max(data.DefaultDelay / 4, 50));
                }
            });
        }
    }
}