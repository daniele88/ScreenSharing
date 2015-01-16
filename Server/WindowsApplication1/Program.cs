using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            System.Threading.Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
            ApartmentState AState = System.Threading.Thread.CurrentThread.GetApartmentState();  
            Application.Run(new MainForm());
        }
    }
}