using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ChapterTool.Forms
{
    public partial class FormPreview : Form
    {
        Point MainPos = Point.Empty;

        public FormPreview(string text,Point pos)
        {
            InitializeComponent();
            cTextBox1.Text = text;
            MainPos = pos;
            base.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }

        public void UpdateText(string text)
        {
            cTextBox1.Text = text;
        }

        private void cTextBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Close();
        }



        private void FormPreview_Load(object sender, EventArgs e)
        {
            MainPos.X = MainPos.X - 230;
            Location = MainPos;
        }

        private void FormPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
