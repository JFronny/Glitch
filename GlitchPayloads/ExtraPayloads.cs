using System.Drawing;
using CC_Functions.W32;

namespace GlitchPayloads
{
    [PayloadClass]
    public class ExtraPayloads
    {
        [Payload(false, 50, 2000)]
        public static void PayloadWindowMove()
        {
            Wnd32 tmp = Wnd32.Foreground;
            Rectangle pos = tmp.Position;
            pos.X += Common.Rnd.Next(-2, 3);
            pos.Y += Common.Rnd.Next(-2, 3);
            tmp.Position = pos;
        }
    }
}