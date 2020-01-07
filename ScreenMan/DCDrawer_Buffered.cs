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
        public Graphics Graphics { get; }
        private readonly BufferedGraphics buffer;
        private readonly IntPtr ptr;
        private readonly Graphics srcGraphics;
        private Drawer _drawerImplementation;

        public DCDrawer_Buffered(IntPtr ptr)
        {
            this.ptr = ptr;
            srcGraphics = Graphics.FromHdc(ptr);
            buffer = BufferedGraphicsManager.Current.Allocate(srcGraphics, Screen.PrimaryScreen.Bounds);
            Graphics = buffer.Graphics;
        }

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
        public Graphics Graphics { get; }
        private readonly IntPtr ptr;

        public DCDrawer_Unbuffered(IntPtr ptr)
        {
            this.ptr = ptr;
            Graphics = Graphics.FromHdc(ptr);
        }

        public void Dispose()
        {
            Graphics.Dispose();
            W32.ReleaseDC(IntPtr.Zero, ptr);
        }
    }
}