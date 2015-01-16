using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace ChatClient
{
    public partial class Form2 : Form
    {
        public Form1 formPrincipale;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 form, string username, string ipAddr, string password, string port)
        {
            InitializeComponent();
            this.formPrincipale = form;
            txtUser.Text = username;
            txtIp.Text = ipAddr;
            txtPassw.Text = password;
            txtPort.Text = port;
        }

        public Form2(Form1 form)
        {
            InitializeComponent();
            this.formPrincipale = form;
        }


        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //for testing
            //txtUser.Text = "albyy";
            //txtPassw.Text = "ciao";
            //txtPort.Text = "1500";
            //txtIp.Text = "130.192.31.225";

            if (formPrincipale.setNick(txtUser.Text) == true && formPrincipale.setPsw(txtPassw.Text) == true  &&  formPrincipale.setIp(txtIp.Text)==true && formPrincipale.setPort(txtPort.Text) == true )
            {
                    formPrincipale.setFlag();
                    this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPassw_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {

        }
    }
}