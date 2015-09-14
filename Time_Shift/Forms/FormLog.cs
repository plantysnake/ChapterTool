using System;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChapterTool
{
    public partial class FormLog : Form
    {


        // Methods
        public FormLog()
        {
            this.InitializeComponent();
            this.InitForm();
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
                this.txtLog.Text = CTLogger.LogText;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void frmLog_Activated(object sender, EventArgs e)
        {
            this.txtLog.Text = CTLogger.LogText;
        }

        private void frmLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            base.Hide();
        }

        private void InitForm()
        {
            base.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            this.Text = string.Format("ChapterTool v{0} -- Log", Assembly.GetExecutingAssembly().GetName().Version);
        }


        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            this.txtLog.Select(this.txtLog.TextLength + 1, 0);
            this.txtLog.ScrollToCaret();
            this.grpLog.Text = string.Format("Log ({0})", this.txtLog.Lines.LongLength);
        }
    }


}