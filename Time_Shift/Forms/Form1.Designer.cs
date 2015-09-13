namespace ChapterTool
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTrans = new System.Windows.Forms.Button();
            this.cbReserveName = new System.Windows.Forms.CheckBox();
            this.Tips = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cbFramCal = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cbRound = new System.Windows.Forms.CheckBox();
            this.btnAUTO = new System.Windows.Forms.Button();
            this.cbMore = new System.Windows.Forms.CheckBox();
            this.cbMul1k1 = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cbShift = new System.Windows.Forms.CheckBox();
            this.cbChapterName = new System.Windows.Forms.CheckBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLog = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.combineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
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
            // textBox1
            // 
            this.textBox1.AcceptsReturn = true;
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.Location = new System.Drawing.Point(12, 84);
            this.textBox1.Margin = new System.Windows.Forms.Padding(12, 4, 3, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(250, 350);
            this.textBox1.TabIndex = 1;
            this.textBox1.TabStop = false;
            this.textBox1.WordWrap = false;
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(318, 36);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 40);
            this.btnSave.TabIndex = 3;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSave.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSave_MouseDown);
            this.btnSave.MouseEnter += new System.EventHandler(this.btnSave_MouseEnter);
            this.btnSave.MouseLeave += new System.EventHandler(this.toolTipRemoveAll);
            // 
            // textBox2
            // 
            this.textBox2.AcceptsReturn = true;
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox2.Location = new System.Drawing.Point(318, 84);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 4, 3, 4);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(250, 350);
            this.textBox2.TabIndex = 6;
            this.textBox2.TabStop = false;
            this.textBox2.WordWrap = false;
            this.textBox2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textBox2_MouseClick);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 23);
            this.label1.TabIndex = 5;
            this.label1.MouseEnter += new System.EventHandler(this.label1_MouseEnter);
            this.label1.MouseLeave += new System.EventHandler(this.toolTipRemoveAll);
            // 
            // btnTrans
            // 
            this.btnTrans.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnTrans.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnTrans.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnTrans.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTrans.Location = new System.Drawing.Point(270, 84);
            this.btnTrans.Margin = new System.Windows.Forms.Padding(4);
            this.btnTrans.Name = "btnTrans";
            this.btnTrans.Size = new System.Drawing.Size(40, 40);
            this.btnTrans.TabIndex = 2;
            this.btnTrans.TabStop = false;
            this.btnTrans.Text = ">>";
            this.btnTrans.UseVisualStyleBackColor = true;
            this.btnTrans.Click += new System.EventHandler(this.btnTrans_Click);
            this.btnTrans.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Color_MouseDown);
            // 
            // cbReserveName
            // 
            this.cbReserveName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbReserveName.Location = new System.Drawing.Point(318, 7);
            this.cbReserveName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbReserveName.Name = "cbReserveName";
            this.cbReserveName.Size = new System.Drawing.Size(180, 21);
            this.cbReserveName.TabIndex = 3;
            this.cbReserveName.TabStop = false;
            this.cbReserveName.Text = "保留原章节名";
            this.cbReserveName.UseVisualStyleBackColor = true;
            this.cbReserveName.CheckedChanged += new System.EventHandler(this.cbReserveName_CheckedChanged);
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
            // cbFramCal
            // 
            this.cbFramCal.AutoSize = true;
            this.cbFramCal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFramCal.Location = new System.Drawing.Point(447, 7);
            this.cbFramCal.Name = "cbFramCal";
            this.cbFramCal.Size = new System.Drawing.Size(72, 21);
            this.cbFramCal.TabIndex = 10;
            this.cbFramCal.TabStop = false;
            this.cbFramCal.Text = "帧数计算";
            this.cbFramCal.UseVisualStyleBackColor = true;
            this.cbFramCal.CheckedChanged += new System.EventHandler(this.cbFramCal_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.SystemColors.Window;
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
            this.comboBox1.Location = new System.Drawing.Point(447, 36);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 25);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.TabStop = false;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // cbRound
            // 
            this.cbRound.AutoSize = true;
            this.cbRound.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbRound.Location = new System.Drawing.Point(318, 7);
            this.cbRound.Name = "cbRound";
            this.cbRound.Size = new System.Drawing.Size(88, 21);
            this.cbRound.TabIndex = 12;
            this.cbRound.TabStop = false;
            this.cbRound.Text = "帧数取整    ";
            this.cbRound.UseVisualStyleBackColor = true;
            this.cbRound.Visible = false;
            this.cbRound.CheckedChanged += new System.EventHandler(this.cbRound_CheckedChanged);
            this.cbRound.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cbRound_MouseDown);
            // 
            // btnAUTO
            // 
            this.btnAUTO.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnAUTO.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnAUTO.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.btnAUTO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAUTO.Location = new System.Drawing.Point(318, 36);
            this.btnAUTO.Name = "btnAUTO";
            this.btnAUTO.Size = new System.Drawing.Size(80, 40);
            this.btnAUTO.TabIndex = 2;
            this.btnAUTO.TabStop = false;
            this.btnAUTO.Text = "AUTO";
            this.btnAUTO.UseVisualStyleBackColor = true;
            this.btnAUTO.Visible = false;
            this.btnAUTO.Click += new System.EventHandler(this.btnAUTO_Click);
            // 
            // cbMore
            // 
            this.cbMore.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMore.AutoSize = true;
            this.cbMore.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cbMore.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.cbMore.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.cbMore.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.cbMore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.cbMore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMore.Font = new System.Drawing.Font("微软雅黑", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMore.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbMore.Location = new System.Drawing.Point(544, 440);
            this.cbMore.MaximumSize = new System.Drawing.Size(24, 24);
            this.cbMore.Name = "cbMore";
            this.cbMore.Size = new System.Drawing.Size(24, 24);
            this.cbMore.TabIndex = 14;
            this.cbMore.TabStop = false;
            this.cbMore.Text = "∨";
            this.cbMore.UseVisualStyleBackColor = false;
            this.cbMore.CheckedChanged += new System.EventHandler(this.cbMore_CheckedChanged);
            // 
            // cbMul1k1
            // 
            this.cbMul1k1.AutoSize = true;
            this.cbMul1k1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMul1k1.Location = new System.Drawing.Point(12, 490);
            this.cbMul1k1.Margin = new System.Windows.Forms.Padding(12, 3, 3, 3);
            this.cbMul1k1.Name = "cbMul1k1";
            this.cbMul1k1.Size = new System.Drawing.Size(177, 21);
            this.cbMul1k1.TabIndex = 15;
            this.cbMul1k1.TabStop = false;
            this.cbMul1k1.Text = "所有章节的开始时间 x 1.001";
            this.cbMul1k1.UseVisualStyleBackColor = true;
            this.cbMul1k1.CheckedChanged += new System.EventHandler(this.contentUpdate);
            this.cbMul1k1.MouseEnter += new System.EventHandler(this.cbMul1k1_MouseEnter);
            this.cbMul1k1.MouseLeave += new System.EventHandler(this.toolTipRemoveAll);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.numericUpDown1.Location = new System.Drawing.Point(419, 491);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(82, 23);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.TabStop = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.contentUpdate);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(318, 494);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "平移章节号";
            // 
            // cbShift
            // 
            this.cbShift.AutoSize = true;
            this.cbShift.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbShift.Location = new System.Drawing.Point(318, 519);
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
            this.cbChapterName.Location = new System.Drawing.Point(12, 519);
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
            this.maskedTextBox1.Location = new System.Drawing.Point(419, 519);
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
            this.comboBox2.BackColor = System.Drawing.SystemColors.Window;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(141, 36);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 25);
            this.comboBox2.TabIndex = 23;
            this.comboBox2.TabStop = false;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.comboBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBox2_MouseDown);
            this.comboBox2.MouseEnter += new System.EventHandler(this.comboBox2_MouseEnter);
            this.comboBox2.MouseLeave += new System.EventHandler(this.toolTipRemoveAll);
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
            this.TSD_1unit.Text = "0.05";
            // 
            // TSD_2unit
            // 
            this.TSD_2unit.Name = "TSD_2unit";
            this.TSD_2unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_2unit.Text = "0.10";
            // 
            // TSD_3unit
            // 
            this.TSD_3unit.Checked = true;
            this.TSD_3unit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TSD_3unit.Name = "TSD_3unit";
            this.TSD_3unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_3unit.Text = "0.15";
            // 
            // TSD_4unit
            // 
            this.TSD_4unit.Name = "TSD_4unit";
            this.TSD_4unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_4unit.Text = "0.20";
            // 
            // TSD_5unit
            // 
            this.TSD_5unit.Name = "TSD_5unit";
            this.TSD_5unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_5unit.Text = "0.25";
            // 
            // TSD_6unit
            // 
            this.TSD_6unit.Name = "TSD_6unit";
            this.TSD_6unit.Size = new System.Drawing.Size(100, 22);
            this.TSD_6unit.Text = "0.30";
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
            this.btnLog.Location = new System.Drawing.Point(516, 490);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(52, 52);
            this.btnLog.TabIndex = 24;
            this.btnLog.TabStop = false;
            this.btnLog.Text = "LOG";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combineToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 48);
            // 
            // combineToolStripMenuItem
            // 
            this.combineToolStripMenuItem.Name = "combineToolStripMenuItem";
            this.combineToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.combineToolStripMenuItem.Text = "合并章节";
            this.combineToolStripMenuItem.Click += new System.EventHandler(this.combineToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(580, 551);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.cbChapterName);
            this.Controls.Add(this.cbShift);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.cbMul1k1);
            this.Controls.Add(this.cbMore);
            this.Controls.Add(this.btnAUTO);
            this.Controls.Add(this.cbRound);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.cbFramCal);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Tips);
            this.Controls.Add(this.cbReserveName);
            this.Controls.Add(this.btnTrans);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnLoad);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Chapter Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.Move += new System.EventHandler(this.Form1_Move);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTrans;
        private System.Windows.Forms.CheckBox cbReserveName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label Tips;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.CheckBox cbFramCal;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cbRound;
        private System.Windows.Forms.Button btnAUTO;
        private System.Windows.Forms.CheckBox cbMore;
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
    }
}

