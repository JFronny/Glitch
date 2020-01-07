using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenLib
{
    public interface Drawer : IDisposable
    {
        Graphics Graphics { get; }
    }

    public class DCDrawer_Buffered : Drawer
    {
        private readonly BufferedGraphics buffer;
        private readonly IntPtr ptr;
        private readonly Graphics srcGraphics;

        public DCDrawer_Buffered(IntPtr ptr)
        {
            this.ptr = ptr;
            srcGraphics = Graphics.FromHdc(ptr);
            buffer = BufferedGraphicsManager.Current.Allocate(srcGraphics, Screen.PrimaryScreen.Bounds);
            Graphics = buffer.Graphics;
        }

        public Graphics Graphics { get; }

        public void Dispose()
        {
            buffer.Render(srcGraphics);
            Graphics.Dispose();
            buffer.Dispose();
            srcGraphics.Dispose();
            W32.ReleaseDC(IntPtr.Zero, ptr);
        }
    }

    public class DCDrawer_Unbuffered : Drawer
    {
        private readonly IntPtr ptr;

        public DCDrawer_Unbuffered(IntPtr ptr)
        {
            this.ptr = ptr;
            Graphics = Graphics.FromHdc(ptr);
        }

        public Graphics Graphics { get; }

        public void Dispose()
        {
            Graphics.Dispose();
            W32.ReleaseDC(IntPtr.Zero, ptr);
        }
    }
}