using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Server
{
    public partial class FrmSelezione : Form
    {
        private bool LeftButtonDown = false;
        private Graphics g;
        private Point ClickPoint, CurrentPoint;
        private Pen MyPen, EraserPen;
        private Rectangle rect;

        public Rectangle getRect() { return rect; }

        public FrmSelezione()
        {
            InitializeComponent();
            this.Top = 0;
            this.Left = 0;
            this.Height = Screen.PrimaryScreen.Bounds.Bottom;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            label1.ForeColor = Color.DarkRed;
            label1.Text = "Choose the desired portion of the screen\nSelect by left mouse button";
            MyPen = new Pen(Color.DarkRed, 2);
            MyPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
            EraserPen = new Pen(Color.White, 4);
            g = CreateGraphics();
        }


        private void Mouse_Down(object sender, MouseEventArgs e)
        {
            LeftButtonDown = true;
            ClickPoint = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            CurrentPoint = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
            rect = new Rectangle(ClickPoint, new Size(0, 0));
        }

        private void Mouse_Up(object sender, MouseEventArgs e)
        {
            LeftButtonDown = false;
            Thread.Sleep(400);
            Close();
        }

        private void Mouse_Move(object sender, MouseEventArgs e)
        {
            if (LeftButtonDown)
            {
                g.DrawRectangle(EraserPen, rect);
                CurrentPoint.X = Cursor.Position.X;
                CurrentPoint.Y = Cursor.Position.Y;
                rect.X = Math.Min(ClickPoint.X, CurrentPoint.X);
                rect.Y = Math.Min(ClickPoint.Y, CurrentPoint.Y);
                rect.Width = Math.Abs(ClickPoint.X - CurrentPoint.X);
                rect.Height = Math.Abs(ClickPoint.Y - CurrentPoint.Y);
                label1.Text = rect.Location.ToString() + "  {X=" + (rect.X + rect.Width).ToString() + ", Y=" + (rect.Y + rect.Height).ToString() + "}";
                g.DrawRectangle(MyPen, rect);
            }
        }

        private void labelCenter(object sender, EventArgs e)
        {
            this.label1.Top = (this.Height - this.label1.Height) / 2;
            this.label1.Left = (this.Width - this.label1.Width) / 2;
        }

        private void FrmSelezione_Load(object sender, EventArgs e)
        {

        }

    }
}