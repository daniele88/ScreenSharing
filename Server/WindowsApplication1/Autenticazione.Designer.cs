namespace Server
{
    partial class Autenticazione
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtpass = new System.Windows.Forms.TextBox();
            this.txtport = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtnick = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtpass
            // 
            this.txtpass.Location = new System.Drawing.Point(142, 79);
            this.txtpass.Name = "txtpass";
            this.txtpass.Size = new System.Drawing.Size(132, 20);
            this.txtpass.TabIndex = 2;
            this.txtpass.UseSystemPasswordChar = true;
            // 
            // txtport
            // 
            this.txtport.Location = new System.Drawing.Point(142, 167);
            this.txtport.Name = "txtport";
            this.txtport.Size = new System.Drawing.Size(132, 20);
            this.txtport.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Porta";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(215, 218);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 38);
            this.button1.TabIndex = 5;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(313, 218);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 38);
            this.button2.TabIndex = 6;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "IP";
            // 
            // txtnick
            // 
            this.txtnick.Enabled = false;
            this.txtnick.Location = new System.Drawing.Point(142, 31);
            this.txtnick.Name = "txtnick";
            this.txtnick.Size = new System.Drawing.Size(132, 20);
            this.txtnick.TabIndex = 1;
            this.txtnick.Text = "(server)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Nickname";
            // 
            // txtIp
            // 
            this.txtIp.FormattingEnabled = true;
            this.txtIp.Location = new System.Drawing.Point(142, 126);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(132, 21);
            this.txtIp.TabIndex = 3;
            // 
            // Autenticazione
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 278);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtnick);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtport);
            this.Controls.Add(this.txtpass);
            this.Name = "Autenticazione";
            this.Text = "Connection settings";
            this.Load += new System.EventHandler(this.Autenticazione_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtpass;
        private System.Windows.Forms.TextBox txtport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtnick;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox txtIp;
    }
}

