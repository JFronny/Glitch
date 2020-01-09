using System;
using GlitchPayloads;
using Misc;

namespace EmissionTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Running Glitch Payload Bundle... (Config: cursor, sd)");
            StandardCommands.GetRunner(StandardCommands.GetMethodInfo(Memz.PayloadCursor), 0, 200).Start();
            StandardCommands.LaunchIncrementor();
        }
    }
}