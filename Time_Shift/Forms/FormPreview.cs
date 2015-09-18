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
        public FormPreview(string text)
        {
            InitializeComponent();
            cTextBox1.Text = text;
            base.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }
    }
}
