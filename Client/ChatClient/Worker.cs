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

namespace ChatClient
{
    public class Worker
    {
        private PictureBox p;
        private Form1 f;
        private SocketforClient s;
        private Bitmap btmp;
        private bool ret;

        private volatile bool _shouldStop;

        public Worker(Form1 f, PictureBox p1, SocketforClient s)
        {
            this.f = f;
            this.s = s;
            this.p = p1;
        }

        public void DoWork()
        {

            while (!_shouldStop)
            {

                try
                {
                    ret = s.Receive(ref btmp);

                    //to change the pictureBox of the form (it was initialized by another thread), so Invoke is needed
                    f.Invoke(new aggiornaPicture(f.change), btmp, ret);
                    Thread.Sleep(30); //update every 30 msec
                }
                catch
                {
                    //Bitmap b = new Bitmap("@C:\\Users\\Alberto\\Desktop\\Immagine.png");
                    //f.Invoke(new aggiornaPicture(f.change), b);
                    Thread.Sleep(30);//alby
                }
            }
        }


        public void RequestStop()
        {
            _shouldStop = true;
        }



        public bool isStopped()
        {
            return _shouldStop;
        }
    }
}


