using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using CC_Functions.W32.Hooks;
using Misc;

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
                    Text = s.Item2.Name,
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
            MethodInfo method = ((Tuple<MethodInfo, PayloadAttribute>) args[0]).Item1;
            PayloadAttribute data = ((Tuple<MethodInfo, PayloadAttribute>) args[0]).Item2;
            Thread th = (Thread) args[1];
            if (th.IsAlive)
            {
                th.Abort();
                args[1] = GetFRunner(method, data);
            }
            if ((bool) args[2])
            {
                args[2] = false;
                btn.BackColor = Color.FromKnownColor(KnownColor.Control);
                ScreenMan.Refresh();
            }
            else
            {
                
                if (data.IsSafe || MessageBox.Show("This payload is considered semi-harmful.\r\nThis means, it should be safe to use, but can still cause data loss or other things you might not want.\r\n\r\nIf you have productive data on your system or signed in to online accounts, it is recommended to run this payload inside a virtual machine in order to prevent potential data loss or changed things you might not want.\r\n\r\nDo you still want to enable it?", 
                        "Glitch", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    th.Start();
                    args[2] = true;
                    btn.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                }
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