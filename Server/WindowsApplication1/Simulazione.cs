using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public partial class Simulazione : Form
    {
        public Simulazione()
        {
            InitializeComponent();
        }

        private void Simulazione_Load(object sender, EventArgs e)
        {
        }

        public PictureBox getPicture() {
            return pictureBox1;
        }
    }
}