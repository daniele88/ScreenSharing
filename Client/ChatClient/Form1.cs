using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace ChatClient
{
    public delegate void aggiornaPicture(Bitmap btmp, bool ret);
    public partial class Form1 : Form
    {
        // Will hold the user name
        private string UserName = "", passw = "";
        private int port=0, port_scr;
        private StreamWriter swSender;
        private StreamReader srReceiver;
        private StreamReader str; // streamreader for clipboard
        private TcpClient tcpServer;
        private TcpClient tcpClip;
        // Needed to update the form with messages from another thread
        private delegate void UpdateLogCallback(string strMessage);
        // Needed to set the form to a "disconnected" state from another thread
        private delegate void CloseConnectionCallback(string strReason);
        private delegate void DisableClipboardCallback(string strReason);
        private delegate void UpdateClipboardCallback(System.Collections.Specialized.StringCollection paths); 
        private Thread thrMessaging;
        private IPAddress ipAddr;
        private bool Connected, record_showing=true, chat=true, first=true, flagVis=false;
        public Form settings;
        public SocketforClient s;

        Worker workerObject;
        Thread workerThread;

        //needed for clipboard 
        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove,IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        IntPtr nextClipboardViewer;

        public void change(Bitmap p, bool ret)
        {
            if (!workerObject.isStopped())
            {
                pictureBox1.Image = p;
                pictureBox1.Refresh();
                if (ret) { label1.Visible = false; pictureBox1.BackColor = Color.Beige; }
                else { if(record_showing==true)label1.Visible = true; pictureBox1.BackColor = Color.Black; }//alby10
            }
        } 

        public Form1()
        {
            nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);
            // On application exit, don't forget to disconnect first
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();
        }

        // The event handler for application exit
        public void OnApplicationExit(object sender, EventArgs e)
        {
            //ChangeClipboardChain(this.Handle, nextClipboardViewer);
            if (Connected == true)
            {
                // Closes the connections, streams, etc.
                string text = "#####";
                swSender.WriteLine(text);
                Connected = false;
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
                tcpClip.Close();
            }

            try
            {
                workerObject.RequestStop();
                workerThread.Join();
            }
            catch { }
        }

        private void InitializeConnection()
        {
            // Parse the IP address from the TextBox into an IPAddress object
            //ipAddr = IPAddress.Parse(txtIp.Text);
            //ipAddr = IPAddress.Loopback;
            // Start a new TCP connections to the chat server
            MD5 md5 = new MD5CryptoServiceProvider();
            string passmd5 = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(passw)));

            // check if the port choosen by the user is free
            /*  It doesn't work for local connections... Uncomment to use in real network!
            IPEndPoint[] tcpConnInfoArray = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (IPEndPoint endpoint in tcpConnInfoArray)
                if (endpoint.Port == port)
                {
                    System.Windows.Forms.MessageBox.Show("Connection failed: the selected port is already used by the system", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            */
            tcpServer = new TcpClient();
            try
            {
                tcpServer.Connect(new IPEndPoint(ipAddr, port));
                
                // Helps us track whether we're connected or not
                Connected = true;
                txtMessage.Enabled = true;
                btnSend.Enabled = true;
                connettiToolStripMenuItem.Text = "Disconnect";
                if (first == true)
                {
                    txtLog.Clear();
                    first = false;
                }
                // Send the desired username and password to the server
                swSender = new StreamWriter(tcpServer.GetStream());
                swSender.WriteLine(UserName);
                swSender.Flush();
                swSender.WriteLine(passmd5);
                swSender.Flush();

                // Start the thread for receiving messages and further communication
                thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
                thrMessaging.Start();
            }
            catch
            {
                //txtLog.AppendText("Error in connecting to server "+ipAddr.ToString()+" !\r\n\r\n");
                MessageBox.Show("Error while trying to connect!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connettiToolStripMenuItem.Text = "Connect";
            }
        }


        private void ReceiveMessages()
        {
            // Receive the response from the server
            srReceiver = new StreamReader(tcpServer.GetStream());
            // If the first character of the response is 1, connection was successful
            string ConResponse = srReceiver.ReadLine();
            // If the first character is a 1, connection was successful
            if (ConResponse[0] == '1')
            {
                // Update the form to tell it we are now connected
                this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { "Connected Successfully!" });
                //if there is data in the clipboard, enable the button to send it
                IDataObject d = Clipboard.GetDataObject();
                if (d!=null && (d.GetDataPresent(DataFormats.Text) || d.GetDataPresent(DataFormats.Text)))
                    bntClipboard.Enabled = true;
                // read the port sent by the server for screen sharing
                port_scr=int.Parse(srReceiver.ReadLine());
                //MessageBox.Show("port is" + port_scr);
                //start socket for screen sharing
                s = new SocketforClient(ipAddr.ToString(), port_scr);

                //to receive clipboard by other users
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 48231);
                tcpClip = new TcpClient();
                tcpClip.Connect(ipEndPoint);
                str = new StreamReader(tcpClip.GetStream());
                Thread clipThread = new Thread(ReceiveClip);
                clipThread.SetApartmentState(ApartmentState.STA);
                clipThread.Start();

                workerObject = new Worker(this, pictureBox1, s);
                workerThread = new Thread(workerObject.DoWork);
                workerThread.Start();
                while (!workerThread.IsAlive);

            }
            else // If the first character is not a 1 (probably a 0), the connection was unsuccessful
            {
                string Reason = "Not Connected: ";
                // Extract the reason out of the response message. The reason starts at the 3rd character
                Reason += ConResponse.Substring(2, ConResponse.Length - 2);
                try
                {
                    // Update the form with the reason why we couldn't connect
                    this.Invoke(new CloseConnectionCallback(this.CloseConnection), new object[] { Reason });
                }
                catch
                {
                    return;
                }
                // Exit the method
                return;
            }
            // While we are successfully connected, read incoming lines from the server
            while (Connected)
            {
                // Show the messages in the log TextBox
                try
                {
                    this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { srReceiver.ReadLine() });
                }
                catch
                {
                    string Reason = "Disconnected by server!";
                    //MessageBox.Show("Disconnessione!");
                    //CloseConnection("Disconnected...");
                    //btnConnect.Text = "Connect";
                    if (Connected)
                    {
                        this.Invoke(new CloseConnectionCallback(this.CloseConnection), new object[] { Reason });
                        connettiToolStripMenuItem.Text = "Connect";
                        this.Invoke(new DisableClipboardCallback(this.DisableClipboard), new object[] { Reason});
                        str.Close();
                        //bntClipboard.Enabled = false;
                    }
                    // Close the objects
                    //Connected = false;
                }
            }
        }


        private void DisableClipboard(string Reason)
        {
            bntClipboard.Enabled = false;
            bntClipboard.Text = Reason;
        }

        private void EnableClipboard(string Reason)
        {
            bntClipboard.Enabled = true;
            bntClipboard.Text = Reason;
        }

        // This method is called from a different thread in order to update the log TextBox
        private void UpdateLog(string strMessage)
        {
            txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Regular);
            // Updates the log with the message
            string search = " says: ";
            int posizione = 0;
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
                txtLog.AppendText(strMessage + "\r\n\r\n");
                txtLog.SelectionColor = Color.Black;
            }
        }

        // Closes a current connection
        private void CloseConnection(string Reason)
        {
            // Show the reason why the connection is ending
            txtLog.SelectionColor = Color.Black;
            txtLog.AppendText(" ");

            txtLog.SelectionColor = Color.Red;
            txtLog.AppendText(Reason + "\r\n\n");
            //txtLog.SelectionColor = Color.Black;alby10
            // Enable and disable the appropriate controls on the form
            //txtIp.Enabled = true;
            //txtUser.Enabled = true;
            txtMessage.Enabled = false;
            btnSend.Enabled = false;
            connettiToolStripMenuItem.Text = "Connect";
            //btnConnect.Text = "Connect";

            // Close the objects
            Connected = false;
            swSender.Close();
            srReceiver.Close();
            tcpServer.Close();

            try
            {
                workerObject.RequestStop();
                workerThread.Join(1000);//never dies??

            }
            catch
            {
                //MessageBox.Show("never dies??");
            }

            workerThread.Abort();
            //MessageBox.Show("disconnected");
            if (record_showing == true) label1.Visible = true;
            pictureBox1.BackColor = Color.Black;


            Bitmap bitm = new Bitmap(Screen.PrimaryScreen.Bounds.Height, Screen.PrimaryScreen.Bounds.Width);
            pictureBox1.Image = bitm;
            pictureBox1.Refresh();
            //alby2 end 
        }

        // Sends the message typed in to the server
        private void SendMessage()
        {
            string fix = txtMessage.Text.Replace("\r\n", " ");
            txtMessage.Text = fix;
            if (txtMessage.Lines.Length >= 1)
            {  
                if (txtMessage.Text == "#####") { txtMessage.Text = " #####"; }

                swSender.WriteLine(fix);
                //tcpServer.GetStream().WriteByte();

                swSender.Flush();
                //txtMessage.Lines = null;
            }
            txtMessage.Clear();
            //txtMessage.Text = "";
        }

        // We want to send the message when the Send button is clicked
        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        // But we also want to send the message once Enter is pressed
        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key is Enter
            if (e.KeyChar == (char)13)
            {
                string[] RichTextBoxLines = txtMessage.Lines;
                //string fix = System.Text.RegularExpressions.Regex.Replace(txtMessage.Text, @"^\s*$\n", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);
                //txtMessage.Text=fix;
                if (txtMessage.Lines.GetLength(0)==2)
                    txtMessage.Lines[0] = RichTextBoxLines[1];
                SendMessage();
            }
            
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void impostazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (flagVis == false)
            {
                settings = new Form2(this);
                settings.ShowDialog();
            }
            else
            {
                settings = new Form2(this, UserName, ipAddr.ToString(), passw, port.ToString());
                settings.ShowDialog();
            }
         }
        
        public bool setNick(string n) {
            if (string.IsNullOrEmpty(n))
            {
                MessageBox.Show("Insert a username!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            UserName = n;
            return true;
        }
        public bool setPsw(string n)
        {
            if (string.IsNullOrEmpty(n))
            {
                MessageBox.Show("Insert a password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            passw = n;
            return true;
        }

        public bool setIp(string n)
        {
            try
            {
                ipAddr = IPAddress.Parse(n);
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
                port = int.Parse(n);
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
            flagVis = true;
        }

        private void connettiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connected == false) //let's connect
            {
                if (first == true && flagVis == false) //open setting
                {
                    //Opening authentication
                    settings = new Form2(this);
                    settings.ShowDialog();
                    //user presses cancel
                    if (flagVis == false)
                        return;
                }

                InitializeConnection();

            }
            else // let's disconnect
            {
                string text = "#####";//to identity disconnection message
                swSender.WriteLine(text);
                CloseConnection("Disconnected at user's request.");
                connettiToolStripMenuItem.Text = "Connect";
                //str.Close();

                //Application.Restart(); ///////
                //workerThread.Abort();

                //tolto tutto..e messo in CloseConnection
                bntClipboard.Enabled = false;
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            //this.txtLog.ForeColor = System.Drawing.Color.Green;
            this.txtLog.Font = new Font(txtLog.SelectionFont, FontStyle.Italic);
            this.txtLog.Text = "To use the program, first you have to connect to a server.\r\nSelect File -> connect, fill in the fields and click ok.\r\nIf you want to change the options later, select File -> settings.\r\n\r\n";
            label1.Text = "Video Not Avaiable";
            label1.Size = new System.Drawing.Size(19, 29);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Visible = true;
            pictureBox1.BackColor = Color.Black;
            labelCenter();
        }

        
        //__________________________________________________________________________________________________

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Connected == true)
            {
                // Closes the connections, streams, etc.
                string text = "#####";//to identity disconnection
                swSender.WriteLine(text);
                Connected = false;
                swSender.Close();
                srReceiver.Close();
                str.Close();
                tcpServer.Close();
            }
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (record_showing == true)
            {
                button2.Enabled = false;
                pictureBox1.Hide();
                button1.Text = "Show server preview";
                label1.Visible = false;
                pictureBox1.BackColor = Color.Beige;
                this.Width = txtLog.Width + 100;
                record_showing = false;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                //labelCenter();
            }
            else
            {
                button2.Enabled = true;
                label1.Visible = true;
                pictureBox1.BackColor = Color.Black;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                pictureBox1.Show();
                button1.Text = "Hide server preview";
                record_showing = true;
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                labelCenter();
            }
        }

        private void labelCenter()
        {
            this.label1.Top = (pictureBox1.Height - this.label1.Height) / 2;
            this.label1.Left = (pictureBox1.Width - this.label1.Width) / 2;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (chat == true)
            {
                txtLog.Hide();
                txtMessage.Hide();
                btnSend.Hide();
                bntClipboard.Hide();
                button2.Text = "Show chat";
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                //pictureBox1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top);
                pictureBox1.Height = this.Height - 125;
                pictureBox1.Top = this.Top + 75;
                pictureBox1.Width = this.Width - 50;
                pictureBox1.Refresh();
                labelCenter();
                chat = false;
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
                txtLog.Show();
                txtMessage.Show();
                btnSend.Show();
                bntClipboard.Show();
                this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
                this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
                pictureBox1.Width = Screen.PrimaryScreen.WorkingArea.Width - txtLog.Width - 100;
                pictureBox1.Top = this.Top + 25;
                pictureBox1.Height = this.Height - 75;
                pictureBox1.Refresh();
                labelCenter();
                button2.Text = "Hide chat";
                chat = true;
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                        if (Connected == true)
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


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            txtLog.SelectionColor = Color.Black;
            txtLog.AppendText(" ");
            
            string testo = "To use the program, first you have to connect to a server.\r\nSelect File -> connect, fill in the fields and click ok.\r\nIf you want to change the options later, select File -> settings.\r\n\r\n";
            this.txtLog.SelectionFont = new Font(txtLog.SelectionFont, FontStyle.Italic);
            txtLog.SelectionColor = Color.Green;
            this.txtLog.AppendText(testo);
            //this.txtLog.SelectionStart = txtLog.TextLength;
            //this.txtLog.SelectionColor = System.Drawing.Color.Green;
        }

        private void txtLog_TextChanged_1(object sender, EventArgs e)
        {
            txtLog.SelectionStart = txtLog.Text.Length;

            txtLog.ScrollToCaret();

            txtLog.Refresh();
        }

        private void ReceiveClip()
        {
            String text;
            System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();
            NetworkStream netstream = tcpClip.GetStream();
            int j;
            bool abort = false;

            while (Connected==true)
            {
                try
                {
                    string what = str.ReadLine();

                    if (what.Substring(0, 4).Equals("File"))
                    {
                        string Reason = "Receving Clipboard...";
                        this.Invoke(new DisableClipboardCallback(this.DisableClipboard), new object[] { Reason });
                        paths.Clear();
                        string qta = what.Substring(4, 1);
                        for (j = 0; j < int.Parse(qta); j++)
                        {
                            string tmp = str.ReadLine();
                            if (tmp.Equals("Abort"))
                            {
                                abort = true;
                                break;
                            }
                            abort = false;
                            string fileName = tmp.Substring(6);
                            byte[] clientData = Convert.FromBase64String(str.ReadLine());
                            BinaryWriter bWrite = new BinaryWriter(File.Open(Path.GetFullPath(@".\File ricevuti\") + fileName, FileMode.Create));
                            bWrite.Write(clientData, 4 + fileName.Length, clientData.Length - 4 - fileName.Length);
                            bWrite.Close();
                            paths.Add(Path.GetFullPath(@".\File ricevuti\") + fileName);                          
                            //SetClipboard(fileName);
                        }
                        Reason = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                        if (abort==false)
                            this.Invoke(new UpdateClipboardCallback(UpdateClipboard), new object[] { paths });
                    }

                    else if (what.Substring(0, 4).Equals("Text")) // text
                    {
                        string Reason = "Receving Clipboard...";
                        this.Invoke(new DisableClipboardCallback(this.DisableClipboard), new object[] { Reason });
                        byte[] bytes = new byte[tcpClip.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        netstream.Read(bytes, 0, (int)tcpClip.ReceiveBufferSize);
                        string textcrc = Encoding.ASCII.GetString(bytes);
                        text = TrimFromZero(textcrc);
                        IDataObject ido = new DataObject();
                        ido.SetData(text);
                        Clipboard.SetDataObject(ido, true);
                        Reason = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                        //MessageBox.Show(text);
                    }

                    else if (what.Substring(0, 4).Equals("Imag")) // bitmap
                    {
                        string Reason = "Receving Clipboard...";
                        this.Invoke(new DisableClipboardCallback(this.DisableClipboard), new object[] { Reason });
                        Stream stm = tcpClip.GetStream();
                        IFormatter formatter = new BinaryFormatter();
                        Bitmap bitm = (Bitmap)formatter.Deserialize(stm);
                        Clipboard.SetImage(bitm);
                        Reason = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                    }
                }

                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                    break;
                }
            }
        }

        private void UpdateClipboard(System.Collections.Specialized.StringCollection paths)
        {
            //System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();
            //paths.Add(Path.GetFullPath(@".\File ricevuti\") + fileName);
            Clipboard.SetFileDropList(paths);
        }

        private string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;

            return input.Substring(0, index);
        }

        private void SendClipFile(object data)
        {
            StreamWriter sw = new StreamWriter(tcpClip.GetStream());
            IDataObject d = (IDataObject)data;

            string Reason = "Sharing Clipboard...";
            this.Invoke(new DisableClipboardCallback(this.DisableClipboard), new object[] { Reason });

            // Contains the file names inside the clipboard
            object fromClipboard = d.GetData(DataFormats.FileDrop, true);
            int qta = 0;
            foreach (string sourceFileName in (Array)fromClipboard)
                qta++;
            try
            {
                sw.WriteLine("File" + qta.ToString() + UserName);
                sw.Flush();
            }
            catch
            {
                Reason = "Share Clipboard";
                this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                return;
            }
            foreach (string sourceFileName in (Array)fromClipboard)
            {
                if (Path.GetFileName(sourceFileName) == "")
                {
                    System.Windows.Forms.MessageBox.Show("Sharing failed: it's impossible to copy a directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bntClipboard.Enabled = false;
                    return;
                }

                try
                {
                    FileInfo fleMembers = new FileInfo(sourceFileName);
                    float size = (float)(fleMembers.Length / 1024 / 1024); //MB
                    if (size > 50)
                    {
                        System.Windows.Forms.MessageBox.Show("It's impossible to send " + sourceFileName + " because the file is too big", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                catch //trying to share a directory!!
                {
                }

                try
                {
                    // Converting file in string
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(Path.GetFileName(sourceFileName));
                    byte[] fileData = File.ReadAllBytes(sourceFileName);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    BitConverter.GetBytes(fileNameByte.Length).CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                    // Sending to server
                    try
                    {
                        sw.WriteLine("File: " + Path.GetFileName(sourceFileName));
                        sw.Flush();
                        sw.WriteLine(Convert.ToBase64String(clientData));
                        sw.Flush();
                    }
                    catch
                    {
                        Reason = "Share Clipboard";
                        this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //Reason = "Share Clipboard";
                    //this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
                    //this.Invoke(new DisableClipboardCallback(this.TextClipboard), new object[] { Reason });
                    System.Windows.Forms.MessageBox.Show("Sharing failed: it's impossible to copy a directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    sw.WriteLine("Abort");
                    sw.Flush();
                }
            }
            Reason = "Share Clipboard";
            this.Invoke(new DisableClipboardCallback(this.EnableClipboard), new object[] { Reason });
        }

        private void bntClipboard_Click(object sender, EventArgs e)
        {
            string strclip;
            IDataObject d = Clipboard.GetDataObject();

            StreamWriter sw = new StreamWriter(tcpClip.GetStream());

            if (d.GetDataPresent(DataFormats.Text))  //sending text
            {
                try
                {
                    bntClipboard.Enabled = false;
                    bntClipboard.Text = "Sharing Clipboard...";
                    sw.WriteLine("Text"+UserName);
                    sw.Flush();
                    NetworkStream netstream = tcpClip.GetStream();
                    strclip = (string)d.GetData(DataFormats.Text);
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(strclip);
                    netstream.Write(sendBytes, 0, sendBytes.Length);
                    bntClipboard.Enabled = true;
                    bntClipboard.Text = "Share Clipboard";
                    //MessageBox.Show("inviato");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    bntClipboard.Enabled = true;
                    bntClipboard.Text = "Share Clipboard";
                    return;
                }
            }
            else if (d.GetDataPresent(DataFormats.FileDrop, true))  //sendig files
            {
                ParameterizedThreadStart thrSendFile = new ParameterizedThreadStart(SendClipFile);
                Thread thr1 = new Thread(thrSendFile);
                thr1.Start(d);
            }

            else if (Clipboard.ContainsImage())
            {
                bntClipboard.Enabled = false;
                bntClipboard.Text = "Sharing Clipboard...";
                Bitmap img = (Bitmap)Clipboard.GetImage();
                sw.WriteLine("Imag" + UserName);
                sw.Flush();

                IFormatter formatter = new BinaryFormatter();
                try
                {
                    NetworkStream stream1 = tcpClip.GetStream();
                    formatter.Serialize(stream1, img);
                }
                catch (Exception exc)
                {
                    bntClipboard.Enabled = true;
                    bntClipboard.Text = "Share Clipboard";
                    //MessageBox.Show(exc.ToString());
                    return;
                }
                bntClipboard.Enabled = true;
                bntClipboard.Text = "Share Clipboard";
                //Clipboard.Clear();
                //MessageBox.Show("ok!");
                //Clipboard.SetImage(img);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Sending failed: clipboard is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bntClipboard.Enabled = false;
            }
        }

    }
}
