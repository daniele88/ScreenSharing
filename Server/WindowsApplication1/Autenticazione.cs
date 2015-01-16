using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Server
{
    public partial class Autenticazione : Form
    {
        //class used to authenticate user 

        public MainForm mf;
        public bool OpenFromOpzioni;

        /*
        public Autenticazione()
        {
            InitializeComponent();
        }*/


        public Autenticazione(MainForm m, bool flg)
        {
            InitializeComponent();
            setIpAddress();
            mf = m;
            OpenFromOpzioni = flg;

        }

        public Autenticazione(MainForm m, int address, string pass, string port, bool flg)
        {
            InitializeComponent();
            setIpAddress();
            mf = m;
            txtpass.Text = pass;
            txtport.Text = port;
            txtIp.SelectedIndex = address;
            OpenFromOpzioni = flg;

        }


        private void button1_Click(object sender, EventArgs e)
        {

            //for testing
            //txtnick.Text = "alby";
            //txtpass.Text = "ciao";
            //txtport.Text = "1500";

            
            if (mf.setNick(txtnick.Text)==true && mf.setPsw(txtpass.Text)==true && mf.setPort(txtport.Text)==true)
            {
                if (txtIp.SelectedIndex == -1)
                    MessageBox.Show("Select an ip address!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    mf.setIp(txtIp.SelectedItem.ToString());
                    mf.setIndexAddr(txtIp.SelectedIndex);
                    mf.setFlag();
                    this.Close();
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setIpAddress()
        {
            string myHost = System.Net.Dns.GetHostName();

            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

            // Loop through all IP addresses and display each of them

            foreach (System.Net.IPAddress myIP in myIPs.AddressList)
            {

                if (myIP.AddressFamily.ToString() == System.Net.Sockets.ProtocolFamily.InterNetwork.ToString())
                {
                    //MessageBox.Show(myIP.ToString());
                    txtIp.Items.Add(myIP.ToString());
                }
            }
            txtIp.DropDownStyle=ComboBoxStyle.DropDownList;
            txtIp.SelectedIndex = -1;
        }

        private void Autenticazione_Load(object sender, EventArgs e)
        {
            this.Left = mf.Left;
            this.Top = mf.Top;

            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }
    }
}