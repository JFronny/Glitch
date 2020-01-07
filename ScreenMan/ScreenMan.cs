using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenLib
{
    public static class ScreenMan
    {
        public static Image CaptureScreen() => CaptureWindow(W32.GetDesktopWindow());

        public static Image CaptureWindow(IntPtr handle)
        {
            IntPtr hdcSrc = W32.GetWindowDC(handle);
            W32.RECT windowRect = new W32.RECT();
            W32.GetWindowRect(handle, ref windowRect);
            IntPtr hdcDest = W32.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = W32.CreateCompatibleBitmap(hdcSrc, windowRect.Width, windowRect.Height);
            IntPtr hOld = W32.SelectObject(hdcDest, hBitmap);
            W32.BitBlt(hdcDest, 0, 0, windowRect.Width, windowRect.Height, hdcSrc, 0, 0, W32.SRCCOPY);
            W32.SelectObject(hdcDest, hOld);
            W32.DeleteDC(hdcDest);
            W32.ReleaseDC(handle, hdcSrc);
            Image img = Image.FromHbitmap(hBitmap);
            W32.DeleteObject(hBitmap);
            return img;
        }

        public static void Draw(Image img)
        {
            using (Drawer drawerBuffered = GetDrawer())
            {
                drawerBuffered.Graphics.DrawImage(img, GetBounds());
            }
        }

        public static Drawer GetDrawer(bool buffer = true)
        {
            IntPtr ptr = W32.GetDC(IntPtr.Zero);
            return buffer ? (Drawer)new DCDrawer_Buffered(ptr) : new DCDrawer_Unbuffered(ptr);
        }

        public static Rectangle GetBounds() => Screen.PrimaryScreen.Bounds;
    }
}