using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;

namespace ChapterTool
{
    public partial class Form2 : Form
    {
        private int poi = 0;
        public Form2()
        {
            InitializeComponent();
            //this.SizeChanged += new System.EventHandler(this.Form2_SizeChanged);
            //this.BackColor = Color.DimGray;// "#252525";
            poi                  = new Random().Next(1, 5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            label1.Text          = AssemblyProduct;
            label2.Text          = "Version "+ AssemblyVersion;
            label3.Text          = System.IO.File.GetLastWriteTime(GetType().Assembly.Location).ToString();
            notifyIcon1.Visible  = false; 
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        private void close()
        {
            while (this.Opacity > 0)
            {
                this.Opacity -= 0.02;
                Thread.Sleep(20);
            }
            CTLogger.Log("关于窗口被关闭");
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e) { if(poi == 1) { close(); } }
        private void button2_Click(object sender, EventArgs e) { if(poi == 2) { close(); } }
        private void button3_Click(object sender, EventArgs e) { if(poi == 3) { close(); } }
        private void button4_Click(object sender, EventArgs e) { if(poi == 4) { close(); } }

        Point mouseOff;
        bool leftFlag;

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y);
                leftFlag = true;
            }
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e) { leftFlag = false; }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);
                Location = mouseSet;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Thread.Sleep(20000);
            this.WindowState = FormWindowState.Minimized;
            CTLogger.Log("关于窗口被最小化");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            base.Hide();
        }
    }
}
