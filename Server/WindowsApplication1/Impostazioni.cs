using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{


    //Class to manage settings
    public partial class Impostazioni : Form
    {
        private MainForm mf;
        private int tipoCattura;
        private bool flagInit;
        private bool disabilita;
        ////Variabile to contain the type of sharing to do
        //1 --> Window
        //2 --> Screen
        //3 --> Full Screen

        private string Window_desc = "Capture the portion of the screen corresponding to the acrive window in a certain moment.(Default)";
        private string Screen_desc = "Capture the desired portion of the screen";
        private string FullScreen_desc = "Capture the entire screen";

        private int x_s, y_s, w_s, h_s;
        private int fattoreScalaL = Screen.PrimaryScreen.Bounds.Size.Width / 100;
        private int fattoreScalaH = Screen.PrimaryScreen.Bounds.Size.Height / 100;

        public int setTipoCattura(int tipoCatturap)
        {
            tipoCattura = tipoCatturap;
            return 0;
        }

        public int getTipoCattura()
        {
            return tipoCattura;
        }

        public void setScreen(int x, int y, int w, int h)
        {
            x_s = x;
            y_s = y;
            w_s = w;
            h_s = h;
        }

        public Impostazioni(MainForm mf)
        {
            this.mf = mf;
            disabilita = true;
            if (MainForm.StartToolStripMenuItem.Enabled) disabilita = false;
            else if (!mf.connected) disabilita = false;

            InitializeComponent();

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true) setTipoCattura(1);
            if (radioButton2.Checked == true)
            {
                try
                {
                    setScreen(int.Parse(textBox1.Text), int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text));
                }
                catch { }
                setTipoCattura(2);
                mf.setDimensioniParz(x_s, y_s, w_s, h_s);
            }
            if (radioButton3.Checked == true) setTipoCattura(3);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Impostazioni_Load(object sender, EventArgs e)
        {

            radioButton1.Text = "Window (Default)";
            flagInit = true;
            this.Left = mf.Left;
            this.Top = mf.Top;

            richTextBox1.BackColor = this.BackColor;
            setTipoCattura(mf.getTipoCattura());
            Rectangle r = mf.getRect();

            setScreen(r.X, r.Y, r.Width, r.Height);

            if (tipoCattura == 1) { radioButton1.Checked = true; groupBox2.Enabled = false; }
            if (tipoCattura == 2) { radioButton2.Checked = true; groupBox2.Enabled = true; }
            if (tipoCattura == 3) { radioButton3.Checked = true; groupBox2.Enabled = false; }

            toolTip1.SetToolTip(radioButton1, Window_desc);
            toolTip1.SetToolTip(radioButton2, Screen_desc);
            toolTip1.SetToolTip(radioButton3, FullScreen_desc);
            //toolTip1.SetToolTip(trackBar1, trackBar1.Value.ToString());
            //toolTip1.SetToolTip(trackBar2, trackBar2.Value.ToString());
            //toolTip1.SetToolTip(trackBar3, trackBar3.Value.ToString());
            //toolTip1.SetToolTip(trackBar4, trackBar4.Value.ToString());

            //aggiornaTrackbar();

            flagInit = false;

            if (disabilita)
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
            }

            this.AutoSize = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;


        }

        private void aggiornaTrackbar()
        {
            //int val;
            //trackBar1.Value = ((val=x_s / fattoreScalaL) > 100) ? 100 : val;
            //trackBar2.Value = ((val=y_s / fattoreScalaH) > 100) ? 100 : val;
            //trackBar3.Value = ((val=w_s / fattoreScalaL) > 100) ? 100 : val;
            //trackBar4.Value = ((val=h_s / fattoreScalaH) > 100) ? 100 : val;

        }

        private void aggiornaText()
        {
            textBox1.Text = x_s.ToString();
            textBox2.Text = y_s.ToString();
            textBox3.Text = w_s.ToString();
            textBox4.Text = h_s.ToString();
        }

        private void aggiornaText(string s1, string s2, string s3, string s4)
        {
            textBox1.Text = s1;
            textBox2.Text = s2;
            textBox3.Text = s3;
            textBox4.Text = s4;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = Window_desc;
            groupBox2.Enabled = false;
            if (radioButton1.Checked) aggiornaText("", "", "", "");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = Screen_desc;
            groupBox2.Enabled = true;
            if ((radioButton2.Checked) && (!flagInit)) selezionaParziale();
            aggiornaText();


        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = FullScreen_desc;
            groupBox2.Enabled = false;

            if (radioButton3.Checked) aggiornaText(Screen.PrimaryScreen.Bounds.X.ToString(),
                Screen.PrimaryScreen.Bounds.Y.ToString(),
                Screen.PrimaryScreen.Bounds.Size.Width.ToString(),
                Screen.PrimaryScreen.Bounds.Size.Height.ToString());
        }

        private void TrackBar_changed(object sender, EventArgs e)
        {
            TrackBar t = (TrackBar)sender;
            int val = 0;

            if (t.Name == "trackBar1") val = x_s = t.Value * fattoreScalaL;
            if (t.Name == "trackBar2") val = y_s = t.Value * fattoreScalaH;
            if (t.Name == "trackBar3") val = w_s = t.Value * fattoreScalaL;
            if (t.Name == "trackBar4") val = h_s = t.Value * fattoreScalaH;
            toolTip1.SetToolTip(t, val.ToString());
        }

        private void selezionaParziale()
        {
            Rectangle rect;
            FrmSelezione form5 = new FrmSelezione();
            form5.ShowDialog();
            rect = form5.getRect();
            setScreen(rect.X, rect.Y, rect.Width, rect.Height);
            //aggiornaTrackbar();

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            setScreen(200, 200, 500, 500);
            aggiornaText();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            selezionaParziale();
            aggiornaText();
        }



    }
}