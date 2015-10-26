// ****************************************************************************
//Public Domain
// code from http://sourceforge.net/projects/gmkvextractgui/
// ****************************************************************************
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ChapterTool
{
    public partial class FormLog : Form
    {
        public FormLog()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            Text = string.Format("ChapterTool v{0} -- Log", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void frmLog_Activated(object sender, EventArgs e)
        {
            this.txtLog.Text = CTLogger.LogText;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            this.txtLog.Select(this.txtLog.TextLength + 1, 0);
            this.txtLog.ScrollToCaret();
            this.grpLog.Text = string.Format("Log ({0})", this.txtLog.Lines.LongLength);
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
                Clipboard.SetData(DataFormats.UnicodeText, this.txtLog.SelectedText);
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
                txtLog.Text = CTLogger.LogText;
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
            base.Hide();
        }
    }
}