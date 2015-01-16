using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;



[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;        // x position of upper-left corner
    public int Top;         // y position of upper-left corner
    public int Right;       // x position of lower-right corner
    public int Bottom;      // y position of lower-right corner
}

public struct POINT { public Int32 x, y; }
public struct CURSORINFO { public Int32 cbSize, flags; public IntPtr hCursor; public POINT ptScreenPos; }
public struct ICONINFO { public bool fIcon; public Int32 xHotspot, yHotspot; public IntPtr hbmMask, hbmColor; }

public class WIN32_API
{
    public struct SIZE
    {
        public int cx;
        public int cy;
    }
    public const int SRCCOPY = 13369376;
    public const int SM_CXSCREEN = 0;
    public const int SM_CYSCREEN = 1;

    [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
    public static extern IntPtr DeleteDC(IntPtr hDc);

    [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
    public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
    public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
    public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

    [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", EntryPoint = "GetDC")]
    public static extern IntPtr GetDC(IntPtr ptr);

    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(int abc);

    [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
    public static extern IntPtr GetWindowDC(Int32 ptr);

    [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

    /*
     [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
    public static extern IntPtr DeleteObject(IntPtr hDc);
     
     */

    [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
    public static extern bool GetCursorInfo(out CURSORINFO pci);

    [DllImport("user32.dll", EntryPoint = "GetCursor")]
    public static extern IntPtr GetCursor();

    [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
    public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

    [DllImport("user32.dll", EntryPoint = "CopyIcon")]
    public static extern IntPtr CopyIcon(IntPtr hIcon);


}
