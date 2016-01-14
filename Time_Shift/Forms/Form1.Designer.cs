namespace ChapterTool.Forms
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTrans = new System.Windows.Forms.Button();
            this.cbAutoGenName = new System.Windows.Forms.CheckBox();
            this.Tips = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cbRound = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_0unit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TSD_1unit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_2unit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_3unit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_4unit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_5unit = new System.Windows.Forms.ToolStripMenuItem();
            this.TSD_6unit = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMul1k1 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cbShift = new System.Windows.Forms.CheckBox();
            this.cbChapterName = new System.Windows.Forms.CheckBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.combineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLog = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTimeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cChapterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFrams = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.savingType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.xmlLang = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnExpand = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Location = new System.Drawing.Point(12, 36);
            this.btnLoad.Margin = new System.Windows.Forms.Padding(12, 4, 3, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(80, 40);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.TabStop = false;
            this.btnLoad.Text = "载入";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(109, 36);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 3;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSave.MouseEnter += new System.EventHandler(this.btnSave_MouseEnter);
            this.btnSave.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            this.btnSave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnSave_MouseUp);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(374, 23);
            this.label1.TabIndex = 5;
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            // 
            // btnTrans
            // 
            this.btnTrans.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnTrans.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnTrans.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnTrans.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTrans.Location = new System.Drawing.Point(538, 9);
            this.btnTrans.Margin = new System.Windows.Forms.Padding(4);
            this.btnTrans.Name = "btnTrans";
            this.btnTrans.Size = new System.Drawing.Size(30, 30);
            this.btnTrans.TabIndex = 2;
            this.btnTrans.TabStop = false;
            this.btnTrans.Text = "↻";
            this.btnTrans.UseVisualStyleBackColor = true;
            this.btnTrans.Click += new System.EventHandler(this.refresh_Click);
            this.btnTrans.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Color_MouseUp);
            // 
            // cbAutoGenName
            // 
            this.cbAutoGenName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbAutoGenName.Location = new System.Drawing.Point(318, 488);
            this.cbAutoGenName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbAutoGenName.Name = "cbAutoGenName";
            this.cbAutoGenName.Size = new System.Drawing.Size(97, 21);
            this.cbAutoGenName.TabIndex = 3;
            this.cbAutoGenName.TabStop = false;
            this.cbAutoGenName.Text = "不使用章节名";
            this.cbAutoGenName.UseVisualStyleBackColor = true;
            this.cbAutoGenName.CheckedChanged += new System.EventHandler(this.cbAutoGenName_CheckedChanged);
            // 
            // Tips
            // 
            this.Tips.Location = new System.Drawing.Point(12, 444);
            this.Tips.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Tips.Name = "Tips";
            this.Tips.Size = new System.Drawing.Size(253, 17);
            this.Tips.TabIndex = 1;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(318, 440);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(12, 5, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(220, 24);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 0;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "24000 / 1001",
            "24000 / 1000",
            "25000 / 1000",
            "30000 / 1001",
            "50000 / 1000",
            "60000 / 1001"});
            this.comboBox1.Location = new System.Drawing.Point(447, 52);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 25);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.TabStop = false;
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            // 
            // cbRound
            // 
            this.cbRound.AutoSize = true;
            this.cbRound.Checked = true;
            this.cbRound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRound.ContextMenuStrip = this.contextMenuStrip1;
            this.cbRound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbRound.Location = new System.Drawing.Point(417, 14);
            this.cbRound.Name = "cbRound";
            this.cbRound.Size = new System.Drawing.Size(76, 21);
            this.cbRound.TabIndex = 12;
            this.cbRound.TabStop = false;
            this.cbRound.Text = "帧数取整 ";
            this.cbRound.UseVisualStyleBackColor = true;
            this.cbRound.CheckedChanged += new System.EventHandler(this.refresh_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSD_0unit,
            this.toolStripSeparator1,
            this.TSD_1unit,
            this.TSD_2unit,
            this.TSD_3unit,
            this.TSD_4unit,
            this.TSD_5unit,
            this.TSD_6unit});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem1.Text = "误差范围";
            this.toolStripMenuItem1.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Accuracy_DropDownItemClicked);
            // 
            // TSD_0unit
            // 
            this.TSD_0unit.Name = "TSD_0unit";
            this.TSD_0unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_0unit.Tag = "0.01";
            this.TSD_0unit.Text = "0.01";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // TSD_1unit
            // 
            this.TSD_1unit.Name = "TSD_1unit";
            this.TSD_1unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_1unit.Tag = "0.05";
            this.TSD_1unit.Text = "0.05";
            // 
            // TSD_2unit
            // 
            this.TSD_2unit.Name = "TSD_2unit";
            this.TSD_2unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_2unit.Tag = "0.10";
            this.TSD_2unit.Text = "0.10";
            // 
            // TSD_3unit
            // 
            this.TSD_3unit.Checked = true;
            this.TSD_3unit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TSD_3unit.Name = "TSD_3unit";
            this.TSD_3unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_3unit.Tag = "0.15";
            this.TSD_3unit.Text = "0.15";
            // 
            // TSD_4unit
            // 
            this.TSD_4unit.Name = "TSD_4unit";
            this.TSD_4unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_4unit.Tag = "0.20";
            this.TSD_4unit.Text = "0.20";
            // 
            // TSD_5unit
            // 
            this.TSD_5unit.Name = "TSD_5unit";
            this.TSD_5unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_5unit.Tag = "0.25";
            this.TSD_5unit.Text = "0.25";
            // 
            // TSD_6unit
            // 
            this.TSD_6unit.Name = "TSD_6unit";
            this.TSD_6unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_6unit.Tag = "0.30";
            this.TSD_6unit.Text = "0.30";
            // 
            // cbMul1k1
            // 
            this.cbMul1k1.AutoSize = true;
            this.cbMul1k1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMul1k1.Location = new System.Drawing.Point(192, 518);
            this.cbMul1k1.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.cbMul1k1.Name = "cbMul1k1";
            this.cbMul1k1.Size = new System.Drawing.Size(117, 21);
            this.cbMul1k1.TabIndex = 15;
            this.cbMul1k1.TabStop = false;
            this.cbMul1k1.Text = "章节时间 x 1.001";
            this.toolTip1.SetToolTip(this.cbMul1k1, "用于DVD Decrypter提取的Chapter");
            this.cbMul1k1.UseVisualStyleBackColor = true;
            this.cbMul1k1.CheckedChanged += new System.EventHandler(this.cbMul1k1_CheckedChanged);
            this.cbMul1k1.MouseEnter += new System.EventHandler(this.cbMul1k1_MouseEnter);
            this.cbMul1k1.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDown1.Location = new System.Drawing.Point(516, 488);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(52, 23);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.TabStop = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(430, 491);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "平移章节号";
            // 
            // cbShift
            // 
            this.cbShift.AutoSize = true;
            this.cbShift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbShift.Location = new System.Drawing.Point(318, 518);
            this.cbShift.Name = "cbShift";
            this.cbShift.Size = new System.Drawing.Size(96, 21);
            this.cbShift.TabIndex = 18;
            this.cbShift.TabStop = false;
            this.cbShift.Text = "平移所有时间";
            this.cbShift.UseVisualStyleBackColor = true;
            this.cbShift.CheckedChanged += new System.EventHandler(this.cbShift_CheckedChanged);
            // 
            // cbChapterName
            // 
            this.cbChapterName.AutoSize = true;
            this.cbChapterName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbChapterName.Location = new System.Drawing.Point(192, 488);
            this.cbChapterName.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.cbChapterName.Name = "cbChapterName";
            this.cbChapterName.Size = new System.Drawing.Size(120, 21);
            this.cbChapterName.TabIndex = 21;
            this.cbChapterName.TabStop = false;
            this.cbChapterName.Text = "自定义章节名模板";
            this.cbChapterName.UseVisualStyleBackColor = true;
            this.cbChapterName.CheckedChanged += new System.EventHandler(this.cbChapterName_CheckedChanged);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.AsciiOnly = true;
            this.maskedTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.maskedTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maskedTextBox1.Culture = new System.Globalization.CultureInfo("");
            this.maskedTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.maskedTextBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.maskedTextBox1.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.maskedTextBox1.Location = new System.Drawing.Point(423, 516);
            this.maskedTextBox1.Mask = "00:00:00.000";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.PromptChar = 'T';
            this.maskedTextBox1.Size = new System.Drawing.Size(82, 23);
            this.maskedTextBox1.TabIndex = 22;
            this.maskedTextBox1.TabStop = false;
            this.maskedTextBox1.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.maskedTextBox1_MaskInputRejected);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.comboBox2.ContextMenuStrip = this.contextMenuStrip2;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(318, 51);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 25);
            this.comboBox2.TabIndex = 23;
            this.comboBox2.TabStop = false;
            this.comboBox2.SelectionChangeCommitted += new System.EventHandler(this.comboBox2_SelectionChangeCommitted);
            this.comboBox2.MouseEnter += new System.EventHandler(this.comboBox2_MouseEnter);
            this.comboBox2.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combineToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(125, 26);
            // 
            // combineToolStripMenuItem
            // 
            this.combineToolStripMenuItem.Name = "combineToolStripMenuItem";
            this.combineToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.combineToolStripMenuItem.Text = "合并章节";
            this.combineToolStripMenuItem.Click += new System.EventHandler(this.combineToolStripMenuItem_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "请设置所要保存的位置";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "打开文件";
            // 
            // btnLog
            // 
            this.btnLog.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLog.Location = new System.Drawing.Point(516, 516);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(52, 23);
            this.btnLog.TabIndex = 24;
            this.btnLog.TabStop = false;
            this.btnLog.Text = "LOG";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cOrder,
            this.cTimeCode,
            this.cChapterName,
            this.cFrams});
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.Highlight;
            this.dataGridView1.Location = new System.Drawing.Point(12, 83);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(556, 351);
            this.dataGridView1.TabIndex = 25;
            this.dataGridView1.TabStop = false;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView1_UserDeletingRow);
            // 
            // cOrder
            // 
            this.cOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cOrder.Frozen = true;
            this.cOrder.HeaderText = "#";
            this.cOrder.Name = "cOrder";
            this.cOrder.ReadOnly = true;
            this.cOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cOrder.Width = 21;
            // 
            // cTimeCode
            // 
            this.cTimeCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cTimeCode.HeaderText = " 时间点 ";
            this.cTimeCode.Name = "cTimeCode";
            this.cTimeCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cTimeCode.Width = 57;
            // 
            // cChapterName
            // 
            this.cChapterName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cChapterName.HeaderText = " 章节名 ";
            this.cChapterName.Name = "cChapterName";
            this.cChapterName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cChapterName.Width = 57;
            // 
            // cFrams
            // 
            this.cFrams.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cFrams.HeaderText = " 帧数 ";
            this.cFrams.Name = "cFrams";
            this.cFrams.ReadOnly = true;
            this.cFrams.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cFrams.Width = 45;
            // 
            // savingType
            // 
            this.savingType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.savingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.savingType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.savingType.FormattingEnabled = true;
            this.savingType.Items.AddRange(new object[] {
            "OGM",
            "XML",
            "QPF",
            "Time Codes",
            "TsMuxeR Meta"});
            this.savingType.Location = new System.Drawing.Point(74, 486);
            this.savingType.Name = "savingType";
            this.savingType.Size = new System.Drawing.Size(108, 25);
            this.savingType.TabIndex = 26;
            this.savingType.TabStop = false;
            this.savingType.SelectedIndexChanged += new System.EventHandler(this.savingType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 491);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 27;
            this.label2.Text = "保存格式";
            // 
            // btnPreview
            // 
            this.btnPreview.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnPreview.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnPreview.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreview.Location = new System.Drawing.Point(500, 9);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(30, 30);
            this.btnPreview.TabIndex = 28;
            this.btnPreview.TabStop = false;
            this.btnPreview.Text = "P";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            this.btnPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnPreview_MouseUp);
            // 
            // xmlLang
            // 
            this.xmlLang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.xmlLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.xmlLang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.xmlLang.FormattingEnabled = true;
            this.xmlLang.Location = new System.Drawing.Point(74, 516);
            this.xmlLang.Name = "xmlLang";
            this.xmlLang.Size = new System.Drawing.Size(108, 25);
            this.xmlLang.TabIndex = 29;
            this.xmlLang.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 520);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "XML语言";
            // 
            // btnExpand
            // 
            this.btnExpand.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnExpand.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnExpand.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpand.Location = new System.Drawing.Point(544, 440);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(24, 24);
            this.btnExpand.TabIndex = 31;
            this.btnExpand.TabStop = false;
            this.btnExpand.Text = "∨";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(580, 551);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.xmlLang);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.savingType);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.cbChapterName);
            this.Controls.Add(this.cbShift);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.cbMul1k1);
            this.Controls.Add(this.cbRound);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Tips);
            this.Controls.Add(this.cbAutoGenName);
            this.Controls.Add(this.btnTrans);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Chapter Tool";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(10)))), ((int)(((byte)(143)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.Move += new System.EventHandler(this.Form1_Move);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTrans;
        private System.Windows.Forms.CheckBox cbAutoGenName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label Tips;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cbRound;
        private System.Windows.Forms.CheckBox cbMul1k1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbShift;
        private System.Windows.Forms.CheckBox cbChapterName;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem TSD_1unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_2unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_3unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_4unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_5unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_6unit;
        private System.Windows.Forms.ToolStripMenuItem TSD_0unit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem combineToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox savingType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTimeCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cChapterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFrams;
        private System.Windows.Forms.ComboBox xmlLang;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnExpand;
    }
}

