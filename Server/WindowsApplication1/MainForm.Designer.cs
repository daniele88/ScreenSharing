namespace Server
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connettiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            StartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            endToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opzioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.impostazioniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opzioniConnessioneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurazioneTastiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.bntClipboard = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.opzioniToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(419, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connettiToolStripMenuItem,
            StartToolStripMenuItem,
            endToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // connettiToolStripMenuItem
            // 
            this.connettiToolStripMenuItem.Name = "connettiToolStripMenuItem";
            this.connettiToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.connettiToolStripMenuItem.Text = "Connect";
            this.connettiToolStripMenuItem.Click += new System.EventHandler(this.connettiToolStripMenuItem_Click);
            // 
            // StartToolStripMenuItem
            // 
            StartToolStripMenuItem.Enabled = false;
            StartToolStripMenuItem.Name = "StartToolStripMenuItem";
            StartToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            StartToolStripMenuItem.Text = "Start Recording";
            StartToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // endToolStripMenuItem
            // 
            endToolStripMenuItem.Enabled = false;
            endToolStripMenuItem.Name = "endToolStripMenuItem";
            endToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            endToolStripMenuItem.Text = "Stop Recording";
            endToolStripMenuItem.Click += new System.EventHandler(this.endToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // opzioniToolStripMenuItem
            // 
            this.opzioniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.impostazioniToolStripMenuItem,
            this.opzioniConnessioneToolStripMenuItem,
            this.configurazioneTastiToolStripMenuItem});
            this.opzioniToolStripMenuItem.Name = "opzioniToolStripMenuItem";
            this.opzioniToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.opzioniToolStripMenuItem.Text = "Settings";
            // 
            // impostazioniToolStripMenuItem
            // 
            this.impostazioniToolStripMenuItem.Name = "impostazioniToolStripMenuItem";
            this.impostazioniToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.impostazioniToolStripMenuItem.Text = "Capture screen settings";
            this.impostazioniToolStripMenuItem.Click += new System.EventHandler(this.impostazioniToolStripMenuItem_Click);
            // 
            // opzioniConnessioneToolStripMenuItem
            // 
            this.opzioniConnessioneToolStripMenuItem.Name = "opzioniConnessioneToolStripMenuItem";
            this.opzioniConnessioneToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.opzioniConnessioneToolStripMenuItem.Text = "Connection settings";
            this.opzioniConnessioneToolStripMenuItem.Click += new System.EventHandler(this.opzioniConnessioneToolStripMenuItem_Click);
            // 
            // configurazioneTastiToolStripMenuItem
            // 
            this.configurazioneTastiToolStripMenuItem.Name = "configurazioneTastiToolStripMenuItem";
            this.configurazioneTastiToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.configurazioneTastiToolStripMenuItem.Text = "Keyboard settings";
            this.configurazioneTastiToolStripMenuItem.Click += new System.EventHandler(this.tastiSceltaRapidaToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(79, 22);
            this.toolStripMenuItem2.Text = "?";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(13, 93);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(371, 344);
            this.txtLog.TabIndex = 2;
            this.txtLog.Text = "";
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(325, 472);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(59, 52);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(13, 472);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(291, 52);
            this.txtMessage.TabIndex = 5;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // bntClipboard
            // 
            this.bntClipboard.Enabled = false;
            this.bntClipboard.Location = new System.Drawing.Point(13, 443);
            this.bntClipboard.Name = "bntClipboard";
            this.bntClipboard.Size = new System.Drawing.Size(131, 23);
            this.bntClipboard.TabIndex = 6;
            this.bntClipboard.Text = "Share Clipboard";
            this.bntClipboard.UseVisualStyleBackColor = true;
            this.bntClipboard.Click += new System.EventHandler(this.bntClipboard_Click);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 49);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(35, 13);
            label1.TabIndex = 7;
            label1.Text = "label2";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(419, 548);
            this.Controls.Add(label1);
            this.Controls.Add(this.bntClipboard);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Screen Sharing *server*";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opzioniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem connettiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem impostazioniToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStripMenuItem opzioniConnessioneToolStripMenuItem;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button bntClipboard;
        private System.Windows.Forms.ToolStripMenuItem configurazioneTastiToolStripMenuItem;
        //--> DO NOT REMOVE STATIC
        public static System.Windows.Forms.Label label1;
        public static System.Windows.Forms.ToolStripMenuItem StartToolStripMenuItem;
        public static System.Windows.Forms.ToolStripMenuItem endToolStripMenuItem;
        //--> DO NOT REMOVE STATIC
    }
}