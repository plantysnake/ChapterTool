// ****************************************************************************
// Public Domain
// code from http://sourceforge.net/projects/gmkvextractgui/
// ****************************************************************************
namespace ChapterTool.Forms
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;
    using ChapterTool.Util;

    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            Text = $"ChapterTool v{Assembly.GetExecutingAssembly().GetName().Version} -- Log";
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void frmLog_Activated(object sender, EventArgs e)
        {
            txtLog.Text = Logger.LogText;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            txtLog.Select(txtLog.TextLength + 1, 0);
            txtLog.ScrollToCaret();
            grpLog.Text = $"Log ({txtLog.Lines.LongLength})";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.UnicodeText, txtLog.SelectedText);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                txtLog.Text = Logger.LogText;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            // To avoid getting disposed
            e.Cancel = true;
            Hide();
        }
    }
}