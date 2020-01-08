using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using CC_Functions.W32.Hooks;
using GlitchPayloads;
using ScreenLib;

namespace Glitch
{
    public partial class RunGUI : Form
    {
        public static bool payloadsEnabled = true;
        private readonly List<Button> payloadButtons = new List<Button>();

        public RunGUI()
        {
            InitializeComponent();
            Program.payloads.ForEach(s =>
            {
                Button btn = new Button
                {
                    Text = s.GetPayloadName(),
                    Width = 200,
                    Height = 30,
                    BackColor = Color.FromKnownColor(KnownColor.Control),
                    Tag = new object[] {s, new Thread(() => s.Invoke(null, new object[0])), false}
                };
                payloadButtons.Add(btn);
                btnPanel.Controls.Add(btn);
                btn.Click += PayloadButtonClick;
            });
            new KeyboardHook().OnKeyPress += e =>
            {
                if (e.Key == Keys.Escape)
                    if (KeyboardReader.IsKeyDown(Keys.LShiftKey))
                    {
                        payloadsEnabled = !payloadsEnabled;
                        Text =
                            $"CC24 Glitch - Payloads are {(payloadsEnabled ? "enabled" : "disabled")}. Press LShift+Esc to toggle";
                        payloadButtons.ForEach(btn =>
                        {
                            object[] tmp = (object[]) btn.Tag;
                            tmp[2] = !(bool) tmp[2];
                            btn.Tag = tmp;
                            PayloadButtonClick(btn, new EventArgs());
                        });
                    }
            };
        }

        public void PayloadButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            object[] args = (object[]) btn.Tag;
            MethodInfo s = (MethodInfo) args[0];
            Thread th = (Thread) args[1];
            if (th.IsAlive)
            {
                th.Abort();
                args[1] = new Thread(() => s.Invoke(null, new object[0]));
            }
            if ((bool) args[2])
            {
                args[2] = false;
                btn.BackColor = Color.FromKnownColor(KnownColor.Control);
                ScreenMan.Refresh();
            }
            else
            {
                if (payloadsEnabled)
                    th.Start();
                args[2] = true;
                btn.BackColor = Color.FromKnownColor(KnownColor.Highlight);
            }
            btn.Tag = args;
        }
    }
}