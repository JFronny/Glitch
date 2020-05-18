using System;
using System.IO;
using System.Media;

namespace Misc
{
    public class Beep
    {
        public static void BeepBeep(int amplitude, int frequency, int duration)
        {
            double a = amplitude * Math.Pow(2, 15) / 1000 - 1;
            double deltaFt = 2 * Math.PI * frequency / 44100.0;

            int samples = 441 * duration / 10;
            int bytes = samples * 4;
            int[] hdr =
            {
                0X46464952, 36 + bytes, 0X45564157, 0X20746D66, 16, 0X20001, 44100, 176400, 0X100004, 0X61746164, bytes
            };
            using MemoryStream ms = new MemoryStream(44 + bytes);
            using BinaryWriter bw = new BinaryWriter(ms);
            for (int I = 0; I < hdr.Length; I++)
                bw.Write(hdr[I]);
            for (int T = 0; T < samples; T++)
            {
                short sample = Convert.ToInt16(a * Math.Sin(deltaFt * T));
                bw.Write(sample);
                bw.Write(sample);
            }
            bw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            using SoundPlayer sp = new SoundPlayer(ms);
            sp.PlaySync();
        }
    }
}