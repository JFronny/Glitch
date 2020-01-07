using System;
using System.Drawing;
using System.Threading;
using CC_Functions.W32;

namespace GlitchPayloads
{
    [Payload]
    public class ExtraPayloads
    {
        [Payload]
        public static void PayloadWindowMove()
        {
            while (true)
                try
                {
                    Thread.Sleep(500);
                    Wnd32 tmp = Wnd32.foreground();
                    Rectangle pos = tmp.position;
                    pos.X += Common.Rnd.Next(-2, 3);
                    pos.Y += Common.Rnd.Next(-2, 3);
                    tmp.position = pos;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
        }
    }
}