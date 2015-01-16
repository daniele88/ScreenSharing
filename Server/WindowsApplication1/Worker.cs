using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;

namespace Server
{

    public class Worker
    {
        private int tipoCattura;
        private volatile bool _shouldStop;
        private int width_s, height_s;
        private Point point_s;
        Pen pen;
        protected static IntPtr m_HBitmap;
        Cursor arrow = Cursors.Arrow;
        Point p;
        private int nMilli;
        private byte[] rgbValuesOld;
        private int contUguali;

        public const Int32 CURSOR_SHOWING = 0x00000001;

        public Worker(int tipo)
        {
            tipoCattura = tipo;

            pen = new Pen(Brushes.Red);
            pen.Width = 2.0F;
            RequestStart();
            nMilli = 30;
            contUguali = 0;
        }

        public void setTipoCattura(int tipo)
        {
            tipoCattura = tipo;
        }

        public void UpdateValuesOfScreen(int p_x, int p_y, int w, int h)
        {
            point_s = new Point(p_x, p_y);
            width_s = w;
            height_s = h;
        }

        // This method will be called when the thread is started.
        public void DoWork()
        {

            while (!_shouldStop)
            {
                p = Cursor.Position;
                Graphics gfxScreenshot = null;
                Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                 Screen.PrimaryScreen.Bounds.Height,
                                 PixelFormat.Format32bppArgb);


                //capture the entire screen
                if (tipoCattura == 3)
                {
                    // Create a graphics object from the bitmap. 
                    gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    // Take the screenshot from the upper left corner to the right bottom corner. 
                    try
                    {
                        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                                    Screen.PrimaryScreen.Bounds.Y,
                                                    0,
                                                    0,
                                                    Screen.PrimaryScreen.Bounds.Size,
                                                    CopyPixelOperation.SourceCopy);
                    }
                    catch
                    {
                        continue;
                    }
                    creaBitmapCursore(ref gfxScreenshot, p.X, p.Y);
                }

                //capture the window actually active
                if (tipoCattura == 1)
                {
                    RECT rct = GetForegroundWindow();

                    Size sz = new Size(Math.Abs(rct.Left - rct.Right), Math.Abs(rct.Bottom - rct.Top));

                    if (rct.Bottom > Screen.PrimaryScreen.WorkingArea.Bottom)
                        sz.Height = Math.Abs(Screen.PrimaryScreen.WorkingArea.Bottom - rct.Top);

                    // Create a graphics object from the bitmap. 
                    gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    // Take the screenshot from the upper left corner to the right bottom corner. 
                    gfxScreenshot.CopyFromScreen(rct.Left,
                                                rct.Top,
                                                0,
                                                0,
                                                sz,
                                                CopyPixelOperation.SourceCopy);

                    gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    if (SetCursor(ref p, rct.Left, rct.Top, sz.Width, sz.Height))

                        creaBitmapCursore(ref gfxScreenshot, p.X, p.Y);
                }

                //capture a portion of the screen
                if (tipoCattura == 2)
                {
                    Size sz = new Size(width_s, height_s);
                    // Create a graphics object from the bitmap. 
                    gfxScreenshot = Graphics.FromImage(bmpScreenshot);
                    // Take the screenshot from the upper left corner to the right bottom corner. 
                    gfxScreenshot.CopyFromScreen(point_s.X,
                                                point_s.Y,
                                                0,
                                                0,
                                                sz,
                                                CopyPixelOperation.SourceCopy);

                    if (SetCursor(ref p, point_s.X, point_s.Y, sz.Width, sz.Height))

                        creaBitmapCursore(ref gfxScreenshot, p.X, p.Y);
                }

                gfxScreenshot.Dispose();

                //check with the previous bytes array 
                confrontaBitmap(bmpScreenshot);

                ChatServer.Send(bmpScreenshot);
                Thread.Sleep(nMilli);
            }

        }


        private void confrontaBitmap(Bitmap a)
        {
            try
            {

                // Create a new bitmap.
                Bitmap bmp = a;

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);


                if (rgbValuesOld != null)
                {
                    if (ArraysEqual<byte>(rgbValues, rgbValuesOld))
                    {
                        contUguali++;
                        if (contUguali > 70) { nMilli = 300; contUguali = 0; }
                    }
                    else
                    {
                        nMilli = 30;
                        contUguali = 0;
                    }
                }

                rgbValuesOld = rgbValues;
            }
            catch
            {
                nMilli = 30;
            }
         }
            
        
        //check if two arrays are equal
        static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

        private bool SetCursor(ref Point cursor, int x_new, int y_new, int w_new, int h_new)
        {

            if ((cursor.X >= x_new) && (cursor.Y >= y_new) && (cursor.X <= (x_new + w_new)) && (cursor.Y <= (y_new + h_new)))
            {
                cursor.X = cursor.X - x_new;
                cursor.Y = cursor.Y - y_new;
                return true;
            }
            return false;
        }

        //
        private void creaBitmapCursore(ref Graphics g, int cursorX, int cursorY)
        {

            /*
            Bitmap cursorBMP = null;

            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;

            try
            {
                ci.cbSize = Marshal.SizeOf(ci);
                if (WIN32_API.GetCursorInfo(out ci) && (ci.flags == CURSOR_SHOWING))
                {
                    IntPtr hicon = WIN32_API.CopyIcon(ci.hCursor);
                    if (WIN32_API.GetIconInfo(hicon, out icInfo))
                    {
                        cursorBMP = Icon.FromHandle(hicon).ToBitmap();
                        Rectangle r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                        g.DrawImage(cursorBMP, r);
                    }
                }
            }
            catch { }*/

            Rectangle rCursor = new Rectangle(cursorX, cursorY, arrow.Size.Width, arrow.Size.Height);
            arrow.Draw(g, rCursor);
        }



        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void RequestStart()
        {
            _shouldStop = false;
        }


        public static RECT GetForegroundWindow()
        {

            IntPtr hwnd = WIN32_API.GetForegroundWindow();

            RECT rct;

            WIN32_API.GetWindowRect(hwnd, out rct);

            return rct;

        }
    }
}

