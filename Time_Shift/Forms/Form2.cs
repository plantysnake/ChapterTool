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
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, "具体作用开发中~", "现在完全没用啦", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
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


        //from http://www.sukitech.com/?p=948
        private Point startPoint;
        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = new Point(-e.X , -e.Y);
            //startPoint = new Point(-e.X + SystemInformation.FrameBorderSize.Width, -e.Y - SystemInformation.FrameBorderSize.Height);
        }

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = MousePosition;
                mousePos.Offset(startPoint.X, startPoint.Y);
                Location = mousePos;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Thread.Sleep(20000);
            WindowState = FormWindowState.Minimized;
            CTLogger.Log("关于窗口被最小化");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
