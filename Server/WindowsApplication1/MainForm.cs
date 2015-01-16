using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using System.Diagnostics;

namespace Server
{

    public delegate void DisableClipboardCallback(string Reason);

    public partial class MainForm : Form
    {
        private string nick, password;
        private IPAddress addr;
        private TcpListener tcpL;
        private int imgport, index_addr;
        private int porta;
        private int flagVis;

        private static int tipoCattura;
        public Form af;
        
        public static Worker workerObject;
        public static Thread workerThread;
        private static int w_s, h_s, x_s, y_s;

        private ChatServer mainServer;
        public bool connected, first = true;
        private delegate void UpdateStatusCallback(string strMessage);

        public static Keys kstart;
        public static Keys kend;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;


        // The event handler for application exit
        public void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                if (endToolStripMenuItem.Enabled)
                {
                    workerObject.RequestStop();
                    workerThread.Join();
                }
            }
            catch { }
        }

        //for clipboard
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        public MainForm()
        {
            InitializeComponent();
            //this.Width = Screen.PrimaryScreen.Bounds.Width/ 3;
            //this.Height = Screen.PrimaryScreen.Bounds.Height / 4;
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            this.setDimensioniParz(50, 50, 500, 500);

            flagVis = 0;

            //variable to manage what to share
            tipoCattura = 1;
            this.setDimensioniParz(50, 50, 500, 500);
            this.setTasti(Keys.Up, Keys.Down);
            _hookID = SetHook(_proc);
            workerObject = new Worker(tipoCattura);
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            this.AutoSize = true;
            this.MaximizeBox = false;
            //this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

        }

        public void setTasti(Keys start, Keys end)
        {
            kstart = start;
            kend = end;
        }

        public int getTipoCattura()
        {
            return tipoCattura;
        }

        void FormB_FormClosed(object sender, FormClosedEventArgs e)
        {

            Autenticazione frm = (Autenticazione)sender;

            if ((flagVis == 1) && (!frm.OpenFromOpzioni))
            {
                StartToolStripMenuItem.Enabled = true;
                endToolStripMenuItem.Enabled = false;
            }
        }

        void FormImpostazioni_FormClosed(object sender, FormClosedEventArgs e)
        {
            Impostazioni imp = (Impostazioni)sender;
            tipoCattura = imp.getTipoCattura();
            _hookID = SetHook(_proc);
        }

        public bool setNick(string n)
        {
            if (string.IsNullOrEmpty(n))
            {
                MessageBox.Show("Insert a username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            nick = n;
            return true;
        }
        public bool setPsw(string n)
        {
            if (string.IsNullOrEmpty(n))
            {
                MessageBox.Show("Insert a password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            password = n;
            return true;
        }
        public bool setIp(string n)
        {
            try
            {
                addr = IPAddress.Parse(n);
                return true;
            }
            catch
            {
                MessageBox.Show("Insert a valid Ip address!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public bool setPort(string n)
        {
            try
            {
                porta = int.Parse(n);
                return true;
            }
            catch
            {
                MessageBox.Show("Insert a valid port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void setFlag()
        {
            flagVis = 1;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //closing server
            if (connected)
            {
                mainServer.setRunning();
                tcpL.Stop();
                mainServer.closeClip();
            }
            UnhookWindowsHookEx(_hookID);
            this.Close();
        }

        public static void start()
        {
            if (StartToolStripMenuItem.Enabled)
            {
                try
                {
                    workerObject.UpdateValuesOfScreen(x_s, y_s, w_s, h_s);
                    workerObject.setTipoCattura(tipoCattura);
                    workerObject.RequestStart();

                    workerThread = new Thread(workerObject.DoWork);
                    workerThread.Start();
                    att();
                    while (!workerThread.IsAlive) ;
                    endToolStripMenuItem.Enabled = true;
                    StartToolStripMenuItem.Enabled = false;

                }
                catch
                {
                    //MessageBox.Show("Attenzione!Impossibile attivare il comando start da questa finestra.");
                }
            }
        }

        private static void att()
        {
            label1.ForeColor = Color.Green;
            label1.Text = "CAPTURE: active";
        }
        private static void dis()
        {
            label1.ForeColor = Color.Red;
            label1.Text = "CAPTURE: disabled";
        }

        public static void end()
        {
            if (endToolStripMenuItem.Enabled)
            {
                try
                {
                    workerObject.RequestStop();
                    //workerThread.Join();
                    endToolStripMenuItem.Enabled = false;
                    StartToolStripMenuItem.Enabled = true;
                    dis();
                }
                catch
                {
                    //MessageBox.Show("Attenzione!Impossibile attivare il comando end da questa finestra.");
                }
            }
        }

        //Open monitor sharing
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            endToolStripMenuItem.Enabled = true;
            start();
        }

        //closing monitor sharing
        private void endToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartToolStripMenuItem.Enabled = true;
            end();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Create a new instance of the ChatServer object
            mainServer = new ChatServer(this);
            //startListeningToolStripMenuItem.Text = "Start listening";
            btnSend.Enabled = false;
            txtMessage.Enabled = false;
            this.FormClosing += new FormClosingEventHandler(Form1_Closing);
            this.txtLog.ForeColor = System.Drawing.Color.Green;
            this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Italic);
            this.txtLog.Text = "To use the program you have to let clients connect to you.\r\nSelect File -> Connect, fill in the fields and click ok.\r\nOnce connected you can share your desktop through File -> Start Recording, if you want to change what to share, go to Options -> Recording Options\r\n\r\n";
            label1.Size = new System.Drawing.Size(15, 17);
            dis();
            label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        private void connettiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                if (first == true && flagVis == 0)
                {
                    af = new Autenticazione(this, false);
                    af.ShowDialog();
                    //user aborts
                    if (flagVis == 0)
                        return;
                }
                // check if the port choosen by the user is free
                IPEndPoint[] tcpConnInfoArray = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
                foreach (IPEndPoint endpoint in tcpConnInfoArray)
                    if (endpoint.Port == porta)
                    {
                        System.Windows.Forms.MessageBox.Show("Connection failed: the selected port is already used by the system", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                //looks for a open port for screen sharing
                for (imgport = 1300; (imgport < 65535) && (Array.Find(tcpConnInfoArray, ComparePort) != null); ++imgport)
                {//do nothing
                }
                //MessageBox.Show("la porta " + imgport);
                mainServer.setServer(addr, password, porta.ToString(), imgport);
                // Start listening for connections
                tcpL = mainServer.StartListening();
                //connettiMonitor();
                if (first == true)
                {
                    // Hook the StatusChanged event handler to mainServer_StatusChanged
                    ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
                    txtLog.Clear();
                    first = false;
                }
                this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(" ");
                // Show that we started to listen for connections
                txtLog.SelectionColor = Color.Red;
                txtLog.AppendText("Monitoring for connections...\r\n\r\n");
                connettiToolStripMenuItem.Text = "Disconnetti";
                StartToolStripMenuItem.Enabled = true;
                endToolStripMenuItem.Enabled = false;
                dis();
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
                connected = true;
            }
            else
            {
                end();

                mainServer.setRunning();
                tcpL.Stop();
                mainServer.closeClip();
                connettiToolStripMenuItem.Text = "Connetti";
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(" ");
                txtLog.SelectionColor = Color.Red;
                txtLog.AppendText("Stop listening to connections\r\n\r\n");
                btnSend.Enabled = false;
                bntClipboard.Enabled = false;
                txtMessage.Enabled = false;
                StartToolStripMenuItem.Enabled = false;
                endToolStripMenuItem.Enabled = false;
                connected = false;

                ChatServer.RemoveAllUser();
            }
        }

        private bool ComparePort(IPEndPoint p)
        {
            if (p.Port == imgport)
                return true;
            else return false;
        }

        public void setDimensioniParz(int x, int y, int w, int h)
        {
            x_s = x;
            y_s = y;
            w_s = w;
            h_s = h;
        }


        public Rectangle getRect()
        {
            return new Rectangle(x_s, y_s, w_s, h_s);
        }

        private void impostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(_hookID); 

            if (StartToolStripMenuItem.Enabled == false && endToolStripMenuItem.Enabled == true)
                MessageBox.Show("Note: read-only options. To change the options first stop the current capture.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //Settings form
            Impostazioni iif = new Impostazioni(this);
            iif.Activate();
            iif.Show();
            //iif.setScreen(x_s,y_s,w_s,h_s);

            iif.FormClosed += new FormClosedEventHandler(FormImpostazioni_FormClosed);
        }


        /*

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'S') start();
            if (e.KeyChar == 'E') end();
            base.OnKeyPress(e);
        }
        */

        private void opzioniConnessioneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Authentication
            if (flagVis == 0)
                af = new Autenticazione(this, true);
            else
                af = new Autenticazione(this, index_addr, password, porta.ToString(), true);
            af.Activate();
            af.Show();

            af.FormClosed += new FormClosedEventHandler(FormB_FormClosed);
        }



        //________________________________________________________________________________________________
        public void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainServer.setRunning();
            //if (connected)
            try { tcpL.Stop(); }
            catch { }
            //startListeningToolStripMenuItem.Text = "Start listening";
            //this.Close();
        }
        public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            // Call the method that updates the form
            try
            {
                this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
            }
            catch
            {
                //MessageBox.Show("Disconnessione!");
            }
        }

        private void UpdateStatus(string strMessage)
        {
            // Updates the log with the message
            string search = " says: ";
            int posizione = 0;
            /*if (help == true)
            {
                txtLog.Clear();
                txtLog.Text = testo;
                help = false;
            }*/
            if ((posizione = strMessage.IndexOf(search)) != -1)
            {
                string user = strMessage.Substring(0, posizione);
                string msg = strMessage.Substring(posizione);

                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(" ");

                txtLog.SelectionColor = Color.Blue;
                txtLog.AppendText(user + " ");
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(msg + "\r\n\r\n");
            }
            else
            {
                txtLog.SelectionColor = Color.Black;
                txtLog.AppendText(" ");
                txtLog.SelectionColor = Color.Red;
                this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
                txtLog.AppendText(strMessage + "\r\n\r\n");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string fix = txtMessage.Text.Replace( "\r\n", " ");
            txtMessage.Text = fix;
            if (txtMessage.Lines.Length >= 1)
            {
                //string fix = System.Text.RegularExpressions.Regex.Replace(txtMessage.Text, @"^\s*$\n", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
                mainServer.SendMessage("Server", fix);
                //txtMessage.Lines = null;
            }
            txtMessage.Clear();
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key is Enter
            if (e.KeyChar == (char)13)
            {
                string fix = txtMessage.Text.Replace("\r\n", " ");
                txtMessage.Text = fix;
                if (txtMessage.Lines.Length >= 1)
                {
                    mainServer.SendMessage("Server", fix);
                }
                txtMessage.Clear();
            }
        }


        public void setIndexAddr(int p)
        {
            index_addr = p;
        }

        public void disableClipboard(string Reason)
        {
            bntClipboard.Enabled = false;
            bntClipboard.Text = Reason;
        }

        public void enableClipboard(string Reason)
        {
            bntClipboard.Enabled = true;
            bntClipboard.Text = Reason;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string testo = "To use the program you have to connect to clients.\r\nSelect File -> connect, fill in the fields and click ok.\r\nOnce connected you can share your desktop to clients connected through File -> Start Recording, if you want to change what to share, go to Options -> Recording Options\r\n\r\n";
            //txtLog.Clear();
            this.txtLog.SelectionFont = new Font(txtLog.SelectionFont, FontStyle.Italic);
            txtLog.SelectionColor = Color.Green;
            this.txtLog.AppendText(testo);
            //this.txtLog.SelectionStart = txtLog.TextLength;
            //this.txtLog.SelectionColor = System.Drawing.Color.Green;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;

            txtLog.ScrollToCaret();

            txtLog.Refresh();
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (connected == true)
                    {
                        bntClipboard.Enabled = true;
                    }
                    SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer)
                        nextClipboardViewer = m.LParam;
                    else
                        SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void bntClipboard_Click(object sender, EventArgs e)
        {
            string strclip;
            IDataObject d = Clipboard.GetDataObject();


            if (d.GetDataPresent(DataFormats.Text))  //sending text
            {
                strclip = (string)d.GetData(DataFormats.Text);
                mainServer.SendClipboard(strclip);
            }
            else if (d.GetDataPresent(DataFormats.FileDrop, true))  //sending file
            {
                //object fromClipboard = d.GetData(DataFormats.FileDrop, true);
                ParameterizedThreadStart thrSendFile = new ParameterizedThreadStart(mainServer.SendClipboardFile);
                Thread thr1 = new Thread(thrSendFile);
                thr1.Start(d);
                //mainServer.SendClipboardFile(fromClipboard);
            }
            else if (Clipboard.ContainsImage())
            {
                Bitmap img = (Bitmap)Clipboard.GetImage();
                mainServer.SendClipboardBitmap(img);
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartToolStripMenuItem.Text = "Start Capturing(CTRL+" + kstart.ToString() + ")";
            endToolStripMenuItem.Text = "End Capturing(CTRL+" + kend.ToString() + ")";
        }


        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private void tastiSceltaRapidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSelTasti f = new FrmSelTasti(this);
            f.Activate();
            f.Show();

            f.FormClosed += new FormClosedEventHandler(FrmSelTasti_FormClosed);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);

                    if (kstart == (Keys)vkCode && Keys.Control == Control.ModifierKeys)
                    {
                        //MessageBox.Show(kstart.ToString());
                        start();

                    }
                    if ((Keys)vkCode == kend && Keys.Control == Control.ModifierKeys)
                    {
                        //MessageBox.Show(kend.ToString());
                        end();
                    }
                }
            }
            catch { }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        void FrmSelTasti_FormClosed(object sender, FormClosedEventArgs e)
        {
            FrmSelTasti f = (FrmSelTasti)sender;
            if (f.flag) { UnhookWindowsHookEx(_hookID); _hookID = SetHook(_proc); }

        }
    }
}