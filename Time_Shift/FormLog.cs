using System;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChapterTool
{
    public class FormLog : Form
    {
        // Fields
        private Button btnClose;
        private Button btnCopy;
        private Button btnRefresh;
        private IContainer components = null;
        private GroupBox grpActions;
        private GroupBox grpLog;
        private TableLayoutPanel tlpMain;
        private cTextBox txtLog;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
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

        private void InitializeComponent()
        {
            this.tlpMain = new TableLayoutPanel();
            this.grpLog = new GroupBox();
            this.txtLog = new cTextBox();
            this.grpActions = new GroupBox();
            this.btnRefresh = new Button();
            this.btnCopy = new Button();
            this.btnClose = new Button();
            this.tlpMain.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpActions.SuspendLayout();
            base.SuspendLayout();
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 23f));
            this.tlpMain.Controls.Add(this.grpLog, 0, 0);
            this.tlpMain.Controls.Add(this.grpActions, 0, 1);
            this.tlpMain.Dock = DockStyle.Fill;
            this.tlpMain.Location = new Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 69f));
            this.tlpMain.Size = new Size(0x25c, 0x1f5);
            this.tlpMain.TabIndex = 0;
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = DockStyle.Fill;
            this.grpLog.Location = new Point(3, 3);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new Size(0x256, 0x1aa);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            this.txtLog.BackColor = SystemColors.Window;
            this.txtLog.Dock = DockStyle.Fill;
            this.txtLog.Font = new Font("Consolas", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0xa1);
            this.txtLog.Location = new Point(3, 0x13);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = ScrollBars.Both;
            this.txtLog.Size = new Size(0x250, 0x194);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            this.txtLog.TextChanged += new EventHandler(this.txtLog_TextChanged);
            this.grpActions.Controls.Add(this.btnRefresh);
            this.grpActions.Controls.Add(this.btnCopy);
            this.grpActions.Controls.Add(this.btnClose);
            this.grpActions.Dock = DockStyle.Fill;
            this.grpActions.Location = new Point(3, 0x1b3);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(0x256, 0x3f);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            this.btnRefresh.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnRefresh.Location = new Point(0x129, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(0x5d, 40);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);
            this.btnCopy.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnCopy.Location = new Point(0x18e, 15);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new Size(0x5d, 40);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy Selection";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new EventHandler(this.btnCopy_Click);
            this.btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnClose.Location = new Point(0x1f2, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(0x5d, 40);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            base.AutoScaleDimensions = new SizeF(7f, 15f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x25c, 0x1f5);
            base.Controls.Add(this.tlpMain);
            this.Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0xa1);
            this.MinimumSize = new Size(350, 350);
            base.Name = "frmLog";
            this.Text = "frmLog";
            base.Activated += new EventHandler(this.frmLog_Activated);
            base.FormClosing += new FormClosingEventHandler(this.frmLog_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpActions.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            this.txtLog.Select(this.txtLog.TextLength + 1, 0);
            this.txtLog.ScrollToCaret();
            this.grpLog.Text = string.Format("Log ({0})", this.txtLog.Lines.LongLength);
        }
    }


}