using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace FixLang
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.GetCultureInfo("EN-US").SetCurrent();
            Console.Write("Please enter your desired culture name!\r\n> ");
            CultureInfo.GetCultureInfo(Console.ReadLine()).SetCurrent();
            Console.WriteLine("Press [ENTER] to quit!");
            Console.ReadLine();
        }

        private static void SetCurrent(this CultureInfo culture)
        {
            string lang = culture.LCID.ToString("x8");
            PostMessage(0xffff, 0x0050, IntPtr.Zero, LoadKeyboardLayout(lang, 1));
        }

        [DllImport("user32.dll")]
        static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);
    }
}