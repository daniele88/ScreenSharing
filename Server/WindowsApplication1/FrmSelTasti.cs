using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class FrmSelTasti : Form
    {
        private MainForm mf;
        private Keys kstartold, kendold;
        public bool flag;
        private Keys kstartnew, kendnew;

        public FrmSelTasti(MainForm f)
        {
            InitializeComponent();
            this.mf = f;
        }

        private void FrmSelTasti_Load(object sender, EventArgs e)
        {
            label3.Font = new Font(label1.Font.FontFamily, 10);

            flag = false;
            kstartold = kstartnew= MainForm.kstart;
            kendold = kendnew=MainForm.kend;
            this.Left = mf.Left;
            this.Top = mf.Top;
            for (int i = 1; i <= 26; i++) listBox1.Items.Add( System.Text.ASCIIEncoding.ASCII.GetString(new byte[] { (byte)(i+64) }));
            listBox1.Items.Add("F1");
            listBox1.Items.Add("F2");
            listBox1.Items.Add("F3");
            listBox1.Items.Add("F4");
            listBox1.Items.Add("F5");
            listBox1.Items.Add("F6");
            listBox1.Items.Add("F7");
            listBox1.Items.Add("F8");
            listBox1.Items.Add("F9");
            listBox1.Items.Add("F10");
            listBox1.Items.Add("F11");
            listBox1.Items.Add("F12");
            listBox1.Items.Add("Up");
            listBox1.Items.Add("Down");
            listBox1.Items.Add("Right");
            listBox1.Items.Add("Left");
            listBox1.Items.Add("Space");
            listBox2.Items.Add("CTRL");
            listBox2.Enabled = false;

            listBox1.ClearSelected();
            radioButton1.Checked = true;
            listBox1.SetSelected(listBox1.Items.IndexOf(kstartold.ToString()), true);

            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flag = true;
            if(kstartnew.ToString()!=kendnew.ToString()){
                MainForm.kstart = kstartnew;
                MainForm.kend = kendnew;
                this.Close();}
            else{
                MessageBox.Show(this,"Attenzione! Non è possibile assegnare lo stesso tasto sia per lo start che per l'end!","Waiting",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MainForm.kstart = kstartold;
            MainForm.kend = kendold;
            this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            kstartnew = Keys.Up;
            kendnew = Keys.Down;
            if (radioButton1.Checked)
            {
                listBox1.ClearSelected();
                listBox1.SetSelected(listBox1.Items.IndexOf(kstartnew.ToString()), true);
            }
            if (radioButton2.Checked)
            {
                listBox1.ClearSelected();
                listBox1.SetSelected(listBox1.Items.IndexOf(kendnew.ToString()), true);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
  
                if (radioButton1.Checked)
                {
                    listBox1.ClearSelected();
                    listBox1.SetSelected(listBox1.Items.IndexOf(kstartnew.ToString()), true);
                }
     
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
       
                if (radioButton2.Checked)
                {
                    listBox1.ClearSelected();
                    listBox1.SetSelected(listBox1.Items.IndexOf(kendnew.ToString()), true);
                }
         
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                if (radioButton1.Checked)
                {
                    string s = listBox1.SelectedItem.ToString();
                    KeysConverter k = new KeysConverter();
                    Object o = k.ConvertFromString(s);
                    Keys l = (Keys)o;
                    kstartnew = l;
                }
                if (radioButton2.Checked)
                {
                    string s = listBox1.SelectedItem.ToString();
                    KeysConverter k = new KeysConverter();
                    Object o = k.ConvertFromString(s);
                    Keys l = (Keys)o;
                    kendnew = l;
                }
    
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        
    }
}
