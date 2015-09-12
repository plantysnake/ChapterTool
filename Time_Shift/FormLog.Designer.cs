namespace ChapterTool
{
    partial class FormLog
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new cTextBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpActions.SuspendLayout();
            base.SuspendLayout();
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23f));
            this.tlpMain.Controls.Add(this.grpLog, 0, 0);
            this.tlpMain.Controls.Add(this.grpActions, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69f));
            this.tlpMain.Size = new System.Drawing.Size(0x25c, 0x1f5);
            this.tlpMain.TabIndex = 0;
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(3, 3);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(0x256, 0x1aa);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0xa1);
            this.txtLog.Location = new System.Drawing.Point(3, 0x13);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(0x250, 0x194);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            this.grpActions.Controls.Add(this.btnRefresh);
            this.grpActions.Controls.Add(this.btnCopy);
            this.grpActions.Controls.Add(this.btnClose);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Location = new System.Drawing.Point(3, 0x1b3);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(0x256, 0x3f);
            this.grpActions.TabIndex = 1;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.btnRefresh.Location = new System.Drawing.Point(0x129, 15);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(0x5d, 40);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.btnCopy.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.btnCopy.Location = new System.Drawing.Point(0x18e, 15);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(0x5d, 40);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy Selection";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            this.btnClose.Location = new System.Drawing.Point(0x1f2, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(0x5d, 40);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(0x25c, 0x1f5);
            base.Controls.Add(this.tlpMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0xa1);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            base.Name = "frmLog";
            this.Text = "frmLog";
            base.Activated += new System.EventHandler(this.frmLog_Activated);
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLog_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.grpActions.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        #endregion

		
		private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private cTextBox txtLog;
        //private object AnchorStyles;

        public object DockStyle { get; private set; }
    }
}