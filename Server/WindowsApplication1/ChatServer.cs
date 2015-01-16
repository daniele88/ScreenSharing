using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    // Holds the arguments for the StatusChanged event
    public class StatusChangedEventArgs : EventArgs
    {
        // The argument we're interested in is a message describing the event
        private string EventMsg;

        // Property for retrieving and setting the event message
        public string EventMessage
        {
            get
            {
                return EventMsg;
            }
            set
            {
                EventMsg = value;
            }
        }

        // Constructor for setting the event message
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }

    // This delegate is needed to specify the parameters we're passing with our event
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public delegate void UpdateClipboardCallback(System.Collections.Specialized.StringCollection paths);

    class ChatServer
    {
        // This hash table stores users and connections (browsable by user)
        public static Hashtable htUsers = new Hashtable(30); // 30 users at one time limit
        // This hash table stores connections and users (browsable by connection)
        public static Hashtable htConnections = new Hashtable(30); // 30 users at one time limit
        //Will store connections for screen sharing
        public static Hashtable tcpClientsMonitor = new Hashtable(30);
        public static Hashtable tcpClipboard = new Hashtable(30);
        // Will store the IP address passed to it
        private IPAddress ipAddress;
        private string passw;
        private int porta, porta_scr, num_client = 0;
        //private TcpClient tcpClient;
        // The event and its argument will notify the form when a user has connected, disconnected, send message, etc.
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;

        private TcpListener listenerMonitor;
        private TcpClient clientMonitor;
        private MainForm mf;

        // The constructor sets the IP address to the one retrieved by the instantiating object
        public ChatServer(MainForm mf)
        {
            this.mf = mf;
        }

        // The thread that will hold the connection listener
        private Thread thrListener;

        // The TCP object that listens for connections
        private TcpListener tlsClient;
        private TcpListener ClipboardSocket;

        // Will tell the while loop to keep monitoring for connections
        bool ServRunning = false;

        public void setServer(IPAddress address, string passw, string porta, int imgport)
        {
            ipAddress = address;
            porta_scr = imgport;
            this.passw = passw;
            this.porta = int.Parse(porta);
        }

        // Add the user to the hash tables
        public static void AddUser(TcpClient tcpUser, string strUsername)
        {
            // First add the username and associated connection to both hash tables
            ChatServer.htUsers.Add(strUsername, tcpUser);
            ChatServer.htConnections.Add(tcpUser, strUsername);

            // Tell of the new connection to all other users and to the server form
            SendAdminMessage(htConnections[tcpUser] + " has joined us");
        }

        // Remove the user from the hash tables
        public static void RemoveUser(TcpClient tcpUser)
        {
            // If the user is there
            if (htConnections[tcpUser] != null)
            {
                try
                {
                    // Remove the user from the hash table
                    ChatServer.htUsers.Remove(ChatServer.htConnections[tcpUser]);

                    // First show the information and tell the other users about the disconnection
                    SendAdminMessage(htConnections[tcpUser] + " has left us");

                    ChatServer.tcpClientsMonitor.Remove(ChatServer.htConnections[tcpUser]);
                    ChatServer.tcpClipboard.Remove(ChatServer.htConnections[tcpUser]);
                    ChatServer.htConnections.Remove(tcpUser);
                }
                catch
                {
                }
               
                if (MainForm.StartToolStripMenuItem.Enabled == false)
                {
                    MainForm.workerObject.RequestStop();
                    MainForm.workerThread.Join(1000);
                    MainForm.workerThread.Abort();
                    MainForm.workerObject.RequestStart();
                    MainForm.workerThread = new Thread(MainForm.workerObject.DoWork);
                    MainForm.workerThread.Start();
                }
            }
        }

        // This is called when we want to raise the StatusChanged event
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                // Invoke the delegate
                statusHandler(null, e);
            }
        }

        // Send administrative messages
        public static void SendAdminMessage(string Message)
        {
            StreamWriter swSenderSender;

            // First of all, show in our application who says what
            e = new StatusChangedEventArgs("Administrator: " + Message);
            OnStatusChanged(e);

            // Create an array of TCP clients, the size of the number of users we have
            TcpClient[] tcpClients = new TcpClient[ChatServer.htUsers.Count];
            // Copy the TcpClient objects into the array
            ChatServer.htUsers.Values.CopyTo(tcpClients, 0);
            // Loop through the list of TCP clients
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    // Send the message to the current user in the loop
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine("Administrator: " + Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch // If there was a problem, the user is not there anymore, remove him
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }

        // Send messages from one user to all the others
        public void SendMessage(string From, string Message)
        {
            StreamWriter swSenderSender;

            // First of all, show in our application who says what
            e = new StatusChangedEventArgs(From + " says: " + Message);
            OnStatusChanged(e);

            // Create an array of TCP clients, the size of the number of users we have
            TcpClient[] tcpClients = new TcpClient[ChatServer.htUsers.Count];
            // Copy the TcpClient objects into the array
            ChatServer.htUsers.Values.CopyTo(tcpClients, 0);
            // Loop through the list of TCP clients
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    // Send the message to the current user in the loop
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine(From + " says: " + Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch // If there was a problem, the user is not there anymore, remove him
                {
                    RemoveUser(tcpClients[i]);
                    num_client--;
                }
            }
        }

        public void setRunning()
        {
            ServRunning = false;
            //MessageBox.Show("Disabilitato servRunning!");
        }

        public TcpListener StartListening()
        {

            // Get the IP of the first network device, however this can prove unreliable on certain configurations
            //IPAddress ipaLocal = ipAddress;

            // Create the TCP listener object using the IP of the server and the specified port
            
            //ipAddress = IPAddress.Any;//_________________________________________________________________________
            tlsClient = new TcpListener(ipAddress, porta);

            // Start the TCP listener and listen for connections
            tlsClient.Start();

            // The while loop will check for true in this before checking for connections
            ServRunning = true;

            socketforMonitor();
            socketClipboard();

            // Start the new tread that hosts the listener
            thrListener = new Thread(KeepListening);
            thrListener.Start();
            
            return tlsClient;
        }

        private void socketClipboard()
        {
            ClipboardSocket = new TcpListener(ipAddress, 48231);
            ClipboardSocket.Start();
        }

        private void KeepListening()
        {
            TcpClient tcpClient;
            // While the server is running
            //loop!!!
            while (true)
            {
                //MessageBox.Show("inizio keeplistening!");
                // Accept a pending connection
                try
                {
                    tcpClient = tlsClient.AcceptTcpClient();
                    // Create a new instance of Connection
                    Connection(tcpClient);
                }
                catch
                {
                    break;
                }
            }

            // server is stopping, so delete all active users
            RemoveAllUser();
            //MessageBox.Show("ho finito keeplistening!");
            //newConnection.setRunning();
        }

        //needed for disconnection (menu->disconnect)
        public static void RemoveAllUser()
        {
            try
            {
                ChatServer.htUsers.Clear();
                ChatServer.tcpClientsMonitor.Clear();
                ChatServer.htConnections.Clear();
                tcpClipboard.Clear();
            }
            catch
            {

            }
        }

        // This class handels connections; there will be as many instances of it as there will be connected users
        //TcpClient tcpClient;
        // The thread that will send information to the client
        /*private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;
        private string currUser;
        private string strResponse;*/

        // The constructor of the class takes in a TCP connection
        public void Connection(TcpClient tcpCon)
        {
            // The thread that accepts the client and awaits messages
            //thrSender = new Thread(AcceptClient);
            // The thread calls the AcceptClient() method
            //thrSender.Start();

            //create an object ParameterizedThreadStart
            ParameterizedThreadStart thrSender = new ParameterizedThreadStart(AcceptClient);
            Thread thread = new Thread(thrSender);
            //start the new thread with the parameter needed
            thread.Start(tcpCon);
        }

        public void CloseConnection(TcpClient tcpClient, StreamReader srReceiver, StreamWriter swSender)
        {
            // Close the currently open objects
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
        }

        // Occures when a new client is accepted
        private void AcceptClient(object tcpCon)
        {
            bool finito = false;
            string strResponse;
            TcpClient tcpClient = (TcpClient)tcpCon;
            StreamReader srReceiver = new System.IO.StreamReader(tcpClient.GetStream());
            StreamWriter swSender = new System.IO.StreamWriter(tcpClient.GetStream());

            MD5 md5 = new MD5CryptoServiceProvider();
            string passmd5 = BitConverter.ToString(md5.ComputeHash(ASCIIEncoding.Default.GetBytes(passw)));

            // Read the account information from the client
            string currUser = srReceiver.ReadLine();

            // We got a response from the client
            if (currUser != "")
            {
                if (num_client == 4)
                {
                    swSender.WriteLine("0|Too many clients connected to server. Try again later");
                    swSender.Flush();
                    CloseConnection(tcpClient, srReceiver, swSender);
                    return;
                }
                // Store the user name in the hash table
                if (ChatServer.htUsers.Contains(currUser) == true)
                {
                    // 0 means not connected
                    swSender.WriteLine("0|This username already exists.");
                    swSender.Flush();
                    CloseConnection(tcpClient, srReceiver, swSender);
                    return;
                }
                else if (currUser == "Administrator")
                {
                    // 0 means not connected
                    swSender.WriteLine("0|This username is reserved.");
                    swSender.Flush();
                    CloseConnection(tcpClient, srReceiver, swSender);
                    return;
                }
                // checking password
                else if (!passmd5.Equals(srReceiver.ReadLine()))
                {
                    // 0 means not connected
                    swSender.WriteLine("0|The password is not valid !");
                    swSender.Flush();
                    CloseConnection(tcpClient, srReceiver, swSender);
                    return;
                }
                else
                {
                    // 1 means connected successfully
                    swSender.WriteLine("1");
                    swSender.Flush();

                    //send the port for screen sharing
                    swSender.WriteLine(porta_scr);
                    swSender.Flush();

                    num_client++;
                    // Add the user to the hash tables and start listening for messages from him
                    ChatServer.AddUser(tcpClient, currUser);
                    //lister for connections to screen share
                    AcceptMonitor(currUser);
                    AcceptClipboard(currUser);
                }
            }
            else
            {
                CloseConnection(tcpClient, srReceiver, swSender);
                return;
            }

            try
            {
                // Keep waiting for a message from the user
                while (ServRunning == true)
                {
                    if (tcpClient.GetStream().DataAvailable)
                    {
                        if (((strResponse = srReceiver.ReadLine()) != "#####"))
                        {
                            // If it's invalid, remove the user
                            if (strResponse == null)
                            {
                                ChatServer.RemoveUser(tcpClient);
                                num_client--;
                                //MessageBox.Show("rimosso utente!");
                            }
                            else
                            {
                                // Otherwise send the message to all the other users
                                SendMessage(currUser, strResponse);
                            }
                        }
                        else
                        {
                            finito = true;
                            break;
                        }
                    }
                }

            }
            catch
            {
                // If anything went wrong with this user, disconnect him
                ChatServer.RemoveUser(tcpClient);
                num_client--;
            }
            if (finito == true)
            {
                ChatServer.RemoveUser(tcpClient);
                num_client--;
            }
            //MessageBox.Show("ads chiudo connessione!");
            CloseConnection(tcpClient, srReceiver, swSender);
        }

        private void AcceptClipboard(string currUser)
        {
            TcpClient my_tcp;
            my_tcp = ClipboardSocket.AcceptTcpClient();
            
            tcpClipboard.Add(currUser, my_tcp);

            //create an object ParameterizedThreadStart
            ParameterizedThreadStart threadSender = new ParameterizedThreadStart(ReceiveFile);
            Thread clipthread = new Thread(threadSender);
            clipthread.SetApartmentState(ApartmentState.STA);
            //start the new thread with the parameter needed
            clipthread.Start(my_tcp);
        }

        public void socketforMonitor()
        {
            try
            {
                listenerMonitor = new TcpListener(ipAddress, porta_scr);
                listenerMonitor.Start();
            }
            catch { }
        }

        public void AcceptMonitor(string currUser)
        {
            try
            {
                clientMonitor = listenerMonitor.AcceptTcpClient();
                tcpClientsMonitor.Add(currUser, clientMonitor);
            }
            catch { }
        }


        //takes a Bitmap and send to every client serializing the bitmap
        public static void  Send(Bitmap diff)
        {
            IFormatter formatter = new BinaryFormatter();

            // Create an array of TCP clients, the size of the number of users we have
            TcpClient[] tcpClientM = new TcpClient[ChatServer.tcpClientsMonitor.Count];
            // Copy the TcpClient objects into the array
            ChatServer.tcpClientsMonitor.Values.CopyTo(tcpClientM, 0);
            // Loop through the list of TCP clients
            for (int i = 0; i < tcpClientM.Length; i++)
            {
                try
                {
                    NetworkStream stream1 = tcpClientM[i].GetStream();
                    formatter.Serialize(stream1, diff);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                }
            }

        }

        public void SendClipboard(string text)
        {
            TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
            ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (tcpClients[i] == null)
                    {
                        continue;
                    }
                    // Send the message to the current user in the loop                  
                    StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                    sw.WriteLine("Text");
                    sw.Flush();
                    sw = null;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(text);
                    NetworkStream sendstr = tcpClients[i].GetStream();
                    sendstr.Write(sendBytes, 0, sendBytes.Length);
                }
                catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                {
                    //MessageBox.Show(ex.ToString());
                    continue;
                }
            }
            SendAdminMessage("server just shared his clipboard containing text with us!");
        }

        private void ReceiveFile(object stream)
        {
            byte[] buff = new byte[1024];
            int j;
            string text, user_sharing;
            System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();
            
            TcpClient s = (TcpClient) stream; 
            NetworkStream netStream = s.GetStream();
            StreamReader str = new StreamReader(s.GetStream());
            bool abort= false;

             while (ServRunning==true)
             {
                try
                {
                    string what = str.ReadLine();
                    user_sharing = what.Substring(4);
                    if (what.Substring(0, 4).Equals("File")) //someone is sharing a file-clopboard
                    {
                        string Reason = "Receving Clipboard...";
                        mf.Invoke(new DisableClipboardCallback(mf.disableClipboard), new object[] { Reason });
                        user_sharing = what.Substring(5);
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

                            //send file to others users
                            TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
                            String[] users = new String[ChatServer.htUsers.Count];
                            ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);
                            ChatServer.htUsers.Keys.CopyTo(users, 0);
                            for (int i = 0; i < tcpClients.Length; i++)
                            {
                                // Try sending a message to each
                                try
                                {
                                    // If the message is blank or the connection is null, break out
                                    if (tcpClients[i] == null || user_sharing == users[i])
                                    {
                                        continue;
                                    }
                                    StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                                    sw.WriteLine("File");
                                    sw.Flush();
                                    // Send the message to the current user in the loop
                                    sw.WriteLine("File: " + Path.GetFileName(fileName));
                                    sw.Flush();
                                    sw.WriteLine(Convert.ToBase64String(clientData));
                                    sw.Flush();
                                    sw = null;
                                }
                                catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                                {
                                    //MessageBox.Show(ex.ToString());
                                    continue;
                                }
                            }
                        }
                        if (abort == false)
                        {
                            if (int.Parse(qta) == 1)
                                SendAdminMessage(user_sharing + " just shared his clipboard containing a file with us!");
                            else
                                SendAdminMessage(user_sharing + " just shared his clipboard containing some files with us!");
                            mf.Invoke(new UpdateClipboardCallback(UpdateClipboard), new object[] { paths });
                        }
                        Reason = "Share Clipboard";
                        mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                        //mf.enableClipboard();
                        //MessageBox.Show("file ricevuto!!"+paths.ToString());
                        
                    }
                    else if (what.Substring(0, 4).Equals("Text"))
                    {
                        string Reason = "Receving Clipboard...";
                        mf.Invoke(new DisableClipboardCallback(mf.disableClipboard), new object[] { Reason });

                        byte[] bytes = new byte[s.ReceiveBufferSize];

                        // Read can return anything from 0 to numBytesToRead. 
                        // This method blocks until at least one byte is read.
                        netStream.Read(bytes, 0, (int)s.ReceiveBufferSize);
                        string textcrc = Encoding.ASCII.GetString(bytes);
                        text = TrimFromZero(textcrc);
                        //text = str.ReadLine();
                        TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
                        String[] users = new String[ChatServer.htUsers.Count];
                        ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);
                        ChatServer.htUsers.Keys.CopyTo(users, 0);
                        for (int i = 0; i < tcpClients.Length; i++)
                        {
                            // Try sending a message to each
                            try
                            {
                                // If the message is blank or the connection is null, break out
                                if (tcpClients[i] == null || user_sharing == users[i])
                                {
                                    continue;
                                }
                                // Send the message to the current user in the loop

                                StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                                sw.WriteLine("Text");
                                sw.Flush();
                                sw = null;

                                NetworkStream sendstr = tcpClients[i].GetStream();
                                Byte[] sendBytes = Encoding.ASCII.GetBytes(text);
                                sendstr.Write(sendBytes, 0, sendBytes.Length);
                            }
                            catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                            {
                                //MessageBox.Show(ex.ToString());
                                continue;
                            }
                        }

                        IDataObject ido = new DataObject();
                        ido.SetData(text);
                        Clipboard.SetDataObject(ido, true);
                        Reason = "Share Clipboard";
                        mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                        SendAdminMessage(user_sharing + " just shared his clipboard containing text with us!");
                    }
                    else if (what.Substring(0, 4).Equals("Imag"))
                    {
                        string Reason = "Receving Clipboard...";
                        mf.Invoke(new DisableClipboardCallback(mf.disableClipboard), new object[] { Reason });

                        Stream stm = s.GetStream();
                        IFormatter formatter = new BinaryFormatter();
                        Bitmap bitm = (Bitmap)formatter.Deserialize(stm);

                        TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
                        String[] users = new String[ChatServer.htUsers.Count];
                        ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);
                        ChatServer.htUsers.Keys.CopyTo(users, 0);
                        for (int i = 0; i < tcpClients.Length; i++)
                        {
                            // Try sending a message to each
                            try
                            {
                                // If the message is blank or the connection is null, break out
                                if (tcpClients[i] == null || user_sharing == users[i])
                                {
                                    continue;
                                }
                                // Send the message to the current user in the loop
                                NetworkStream sendstr = tcpClients[i].GetStream();

                                StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                                sw.WriteLine("Imag");
                                sw.Flush();

                                IFormatter myformat = new BinaryFormatter();
                                myformat.Serialize(sendstr, bitm);

                            }
                            catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                            {
                                //MessageBox.Show(ex.ToString());
                                continue;
                            }
                        }
                        Clipboard.SetImage(bitm);
                        Reason = "Share Clipboard";
                        mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                        SendAdminMessage(user_sharing + " just shared his clipboard containing an image with us!");
                    }
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                    break;
                }
                //string sss = "ciao";
                //Clipboard.SetDataObject(Encoding.ASCII.GetBytes(sss));
            }
        }

        private static void UpdateClipboard(System.Collections.Specialized.StringCollection paths)
        {
            //System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();
            //paths.Add(Path.GetFullPath(@".\") + fileName);
            Clipboard.SetFileDropList(paths);
        }

        private string TrimFromZero(string input)
        {
            int index = input.IndexOf('\0');
            if (index < 0)
                return input;
            return input.Substring(0, index);
        }


        public void SendClipboardFile(object d)
        {
            int qta = 0;
            IDataObject f = (IDataObject)d;
            object fromClipboard = f.GetData(DataFormats.FileDrop, true);
            foreach (string sourceFileName in (Array)fromClipboard)
                qta++;
            TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
            ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);

            string Reason = "Sharing Clipboard...";
            mf.Invoke(new DisableClipboardCallback(mf.disableClipboard), new object[] { Reason });
            
            for (int i = 0; i < tcpClients.Length; i++)
            {
                // Try sending a message to each
                try
                {
                    // If the message is blank or the connection is null, break out
                    if (tcpClients[i] == null)
                    {
                        continue;
                    }
                    StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                    sw.WriteLine("File"+qta.ToString());
                    sw.Flush();
                    foreach (string sourceFileName in (Array)fromClipboard)
                    {
                        try
                        {
                            // Conversione del file in stringa
                            byte[] fileNameByte = Encoding.ASCII.GetBytes(Path.GetFileName(sourceFileName));
                            byte[] fileData = File.ReadAllBytes(sourceFileName);
                            byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                            BitConverter.GetBytes(fileNameByte.Length).CopyTo(clientData, 0);
                            fileNameByte.CopyTo(clientData, 4);
                            fileData.CopyTo(clientData, 4 + fileNameByte.Length);
                            // Send the message to the current user in the loop
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
                                mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                                return;
                            }
                        }
                        catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                        {
                            System.Windows.Forms.MessageBox.Show("Sharing failed: it's impossible to copy a directory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Reason = "Share Clipboard";
                            mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                            sw.WriteLine("Abort");
                            sw.Flush();
                            return;
                        }
                    }
                    sw = null;
                }
                catch (Exception ex)
                {
                    // In caso di errori di rete
                    Reason = "Share Clipboard";
                    mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
                    return;
                }
            }
            if (qta==1)
                SendAdminMessage("server just shared his clipboard containing a file with us!");
            else
                SendAdminMessage("server just shared his clipboard containing some files with us!");
            Reason = "Share Clipboard";
            mf.Invoke(new DisableClipboardCallback(mf.enableClipboard), new object[] { Reason });
        }

        public void closeClip()
        {
            ClipboardSocket.Stop();
        }

        public void SendClipboardBitmap(Bitmap bitm)
        {
             TcpClient[] tcpClients = new TcpClient[ChatServer.tcpClipboard.Count];
             String[] users = new String[ChatServer.htUsers.Count];
             ChatServer.tcpClipboard.Values.CopyTo(tcpClients, 0);
             ChatServer.htUsers.Keys.CopyTo(users, 0);
             for (int i = 0; i < tcpClients.Length; i++)
             {
                // Try sending a message to each
                try
                {
                // If the message is blank or the connection is null, break out
                if (tcpClients[i] == null)
                    {
                        continue;
                    }
                // Send the message to the current user in the loop
                NetworkStream sendstr = tcpClients[i].GetStream();

                StreamWriter sw = new StreamWriter(tcpClients[i].GetStream());
                sw.WriteLine("Imag");
                sw.Flush();

                IFormatter myformat = new BinaryFormatter();
                myformat.Serialize(sendstr, bitm);

                }
                catch (Exception ex)// If there was a problem, the user is not there anymore, remove him
                {
                    //MessageBox.Show(ex.ToString());
                    continue;
                }
             }
             SendAdminMessage("server just shared his clipboard containing an image with us!");
        }
    }
}
