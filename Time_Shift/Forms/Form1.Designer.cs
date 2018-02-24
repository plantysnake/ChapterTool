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
            this.loadMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbPath = new System.Windows.Forms.Label();
            this.btnTrans = new System.Windows.Forms.Button();
            this.cbAutoGenName = new System.Windows.Forms.CheckBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cbRound = new System.Windows.Forms.CheckBox();
            this.deviationMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAccuracy = new System.Windows.Forms.ToolStripMenuItem();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lbShift = new System.Windows.Forms.Label();
            this.cbShift = new System.Windows.Forms.CheckBox();
            this.cbChapterName = new System.Windows.Forms.CheckBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.combineMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.combineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cbPostFix = new System.Windows.Forms.CheckBox();
            this.btnLog = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTimeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cChapterName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFrams = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.savingType = new System.Windows.Forms.ComboBox();
            this.lbFormat = new System.Windows.Forms.Label();
            this.btnPreview = new System.Windows.Forms.Button();
            this.xmlLang = new System.Windows.Forms.ComboBox();
            this.lbXmlLang = new System.Windows.Forms.Label();
            this.createZonestMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.creatZonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShiftForwardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertSplitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBoxExpression = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsTips = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tsBtnExpand = new System.Windows.Forms.ToolStripDropDownButton();
            this.loadMenuStrip.SuspendLayout();
            this.deviationMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.combineMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.createZonestMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.ContextMenuStrip = this.loadMenuStrip;
            this.btnLoad.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            resources.ApplyResources(this.btnLoad, "btnLoad");
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.TabStop = false;
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // loadMenuStrip
            // 
            this.loadMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem,
            this.appendToolStripMenuItem});
            this.loadMenuStrip.Name = "loadMenuStrip";
            resources.ApplyResources(this.loadMenuStrip, "loadMenuStrip");
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            resources.ApplyResources(this.reloadToolStripMenuItem, "reloadToolStripMenuItem");
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // appendToolStripMenuItem
            // 
            this.appendToolStripMenuItem.Name = "appendToolStripMenuItem";
            resources.ApplyResources(this.appendToolStripMenuItem, "appendToolStripMenuItem");
            this.appendToolStripMenuItem.Click += new System.EventHandler(this.appendToolStripMenuItem_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.TabStop = false;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnSave.MouseEnter += new System.EventHandler(this.btnSave_MouseEnter);
            this.btnSave.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            this.btnSave.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnSave_MouseUp);
            // 
            // lbPath
            // 
            resources.ApplyResources(this.lbPath, "lbPath");
            this.lbPath.Name = "lbPath";
            this.lbPath.MouseEnter += new System.EventHandler(this.lbPath_MouseEnter);
            this.lbPath.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            // 
            // btnTrans
            // 
            this.btnTrans.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnTrans.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnTrans.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            resources.ApplyResources(this.btnTrans, "btnTrans");
            this.btnTrans.Name = "btnTrans";
            this.btnTrans.TabStop = false;
            this.btnTrans.UseVisualStyleBackColor = true;
            this.btnTrans.Click += new System.EventHandler(this.refresh_Click);
            this.btnTrans.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Color_MouseUp);
            // 
            // cbAutoGenName
            // 
            resources.ApplyResources(this.cbAutoGenName, "cbAutoGenName");
            this.cbAutoGenName.Name = "cbAutoGenName";
            this.cbAutoGenName.TabStop = false;
            this.toolTip1.SetToolTip(this.cbAutoGenName, resources.GetString("cbAutoGenName.ToolTip"));
            this.cbAutoGenName.UseVisualStyleBackColor = true;
            this.cbAutoGenName.CheckedChanged += new System.EventHandler(this.cbAutoGenName_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4"),
            resources.GetString("comboBox1.Items5"),
            resources.GetString("comboBox1.Items6")});
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.TabStop = false;
            this.comboBox1.SelectionChangeCommitted += new System.EventHandler(this.comboBox1_SelectionChangeCommitted);
            // 
            // cbRound
            // 
            resources.ApplyResources(this.cbRound, "cbRound");
            this.cbRound.Checked = true;
            this.cbRound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRound.ContextMenuStrip = this.deviationMenuStrip;
            this.cbRound.Name = "cbRound";
            this.cbRound.TabStop = false;
            this.toolTip1.SetToolTip(this.cbRound, resources.GetString("cbRound.ToolTip"));
            this.cbRound.UseVisualStyleBackColor = true;
            this.cbRound.CheckedChanged += new System.EventHandler(this.refresh_Click);
            // 
            // deviationMenuStrip
            // 
            this.deviationMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAccuracy});
            this.deviationMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(this.deviationMenuStrip, "deviationMenuStrip");
            // 
            // tsmAccuracy
            // 
            this.tsmAccuracy.Name = "tsmAccuracy";
            resources.ApplyResources(this.tsmAccuracy, "tsmAccuracy");
            this.tsmAccuracy.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.Accuracy_DropDownItemClicked);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.WindowText;
            resources.ApplyResources(this.numericUpDown1, "numericUpDown1");
            this.numericUpDown1.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.TabStop = false;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // lbShift
            // 
            resources.ApplyResources(this.lbShift, "lbShift");
            this.lbShift.Name = "lbShift";
            // 
            // cbShift
            // 
            resources.ApplyResources(this.cbShift, "cbShift");
            this.cbShift.Name = "cbShift";
            this.cbShift.TabStop = false;
            this.cbShift.UseVisualStyleBackColor = true;
            this.cbShift.CheckedChanged += new System.EventHandler(this.cbShift_CheckedChanged);
            // 
            // cbChapterName
            // 
            resources.ApplyResources(this.cbChapterName, "cbChapterName");
            this.cbChapterName.Name = "cbChapterName";
            this.cbChapterName.TabStop = false;
            this.toolTip1.SetToolTip(this.cbChapterName, resources.GetString("cbChapterName.ToolTip"));
            this.cbChapterName.UseVisualStyleBackColor = true;
            this.cbChapterName.CheckedChanged += new System.EventHandler(this.cbChapterName_CheckedChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.comboBox2.ContextMenuStrip = this.combineMenuStrip;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBox2, "comboBox2");
            this.comboBox2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.TabStop = false;
            this.comboBox2.SelectionChangeCommitted += new System.EventHandler(this.comboBox2_SelectionChangeCommitted);
            this.comboBox2.MouseEnter += new System.EventHandler(this.comboBox2_MouseEnter);
            this.comboBox2.MouseLeave += new System.EventHandler(this.ToolTipRemoveAll);
            // 
            // combineMenuStrip
            // 
            this.combineMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.combineToolStripMenuItem});
            this.combineMenuStrip.Name = "contextMenuStrip2";
            resources.ApplyResources(this.combineMenuStrip, "combineMenuStrip");
            this.combineMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip2_Closed);
            this.combineMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // combineToolStripMenuItem
            // 
            this.combineToolStripMenuItem.Name = "combineToolStripMenuItem";
            resources.ApplyResources(this.combineToolStripMenuItem, "combineToolStripMenuItem");
            this.combineToolStripMenuItem.Click += new System.EventHandler(this.combineToolStripMenuItem_Click);
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // cbPostFix
            // 
            resources.ApplyResources(this.cbPostFix, "cbPostFix");
            this.cbPostFix.Name = "cbPostFix";
            this.toolTip1.SetToolTip(this.cbPostFix, resources.GetString("cbPostFix.ToolTip"));
            this.cbPostFix.UseVisualStyleBackColor = true;
            // 
            // btnLog
            // 
            this.btnLog.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            resources.ApplyResources(this.btnLog, "btnLog");
            this.btnLog.Name = "btnLog";
            this.btnLog.TabStop = false;
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
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.TabStop = false;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            this.dataGridView1.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView1_RowsRemoved);
            this.dataGridView1.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataGridView1_UserDeletingRow);
            // 
            // cOrder
            // 
            this.cOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cOrder.Frozen = true;
            resources.ApplyResources(this.cOrder, "cOrder");
            this.cOrder.Name = "cOrder";
            this.cOrder.ReadOnly = true;
            this.cOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cTimeCode
            // 
            this.cTimeCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.cTimeCode, "cTimeCode");
            this.cTimeCode.Name = "cTimeCode";
            this.cTimeCode.ReadOnly = true;
            this.cTimeCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cChapterName
            // 
            this.cChapterName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.cChapterName, "cChapterName");
            this.cChapterName.Name = "cChapterName";
            this.cChapterName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cFrams
            // 
            this.cFrams.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.cFrams, "cFrams");
            this.cFrams.Name = "cFrams";
            this.cFrams.ReadOnly = true;
            this.cFrams.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // savingType
            // 
            this.savingType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.savingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.savingType, "savingType");
            this.savingType.FormattingEnabled = true;
            this.savingType.Name = "savingType";
            this.savingType.TabStop = false;
            this.savingType.SelectedIndexChanged += new System.EventHandler(this.savingType_SelectedIndexChanged);
            // 
            // lbFormat
            // 
            resources.ApplyResources(this.lbFormat, "lbFormat");
            this.lbFormat.Name = "lbFormat";
            // 
            // btnPreview
            // 
            this.btnPreview.FlatAppearance.BorderColor = System.Drawing.SystemColors.Highlight;
            this.btnPreview.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.btnPreview.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            resources.ApplyResources(this.btnPreview, "btnPreview");
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.TabStop = false;
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            this.btnPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnPreview_MouseUp);
            // 
            // xmlLang
            // 
            this.xmlLang.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.xmlLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.xmlLang, "xmlLang");
            this.xmlLang.FormattingEnabled = true;
            this.xmlLang.Name = "xmlLang";
            this.xmlLang.TabStop = false;
            this.xmlLang.SelectionChangeCommitted += new System.EventHandler(this.xmlLang_SelectionChangeCommitted);
            // 
            // lbXmlLang
            // 
            resources.ApplyResources(this.lbXmlLang, "lbXmlLang");
            this.lbXmlLang.Name = "lbXmlLang";
            // 
            // createZonestMenuStrip
            // 
            this.createZonestMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creatZonesToolStripMenuItem,
            this.ShiftForwardToolStripMenuItem,
            this.InsertSplitToolStripMenuItem});
            this.createZonestMenuStrip.Name = "createZonestMenuStrip";
            resources.ApplyResources(this.createZonestMenuStrip, "createZonestMenuStrip");
            // 
            // creatZonesToolStripMenuItem
            // 
            this.creatZonesToolStripMenuItem.Name = "creatZonesToolStripMenuItem";
            resources.ApplyResources(this.creatZonesToolStripMenuItem, "creatZonesToolStripMenuItem");
            this.creatZonesToolStripMenuItem.Click += new System.EventHandler(this.creatZonesToolStripMenuItem_Click);
            // 
            // ShiftForwardToolStripMenuItem
            // 
            this.ShiftForwardToolStripMenuItem.Name = "ShiftForwardToolStripMenuItem";
            resources.ApplyResources(this.ShiftForwardToolStripMenuItem, "ShiftForwardToolStripMenuItem");
            this.ShiftForwardToolStripMenuItem.Click += new System.EventHandler(this.ShiftForwardToolStripMenuItem_Click);
            // 
            // InsertSplitToolStripMenuItem
            // 
            this.InsertSplitToolStripMenuItem.Name = "InsertSplitToolStripMenuItem";
            resources.ApplyResources(this.InsertSplitToolStripMenuItem, "InsertSplitToolStripMenuItem");
            this.InsertSplitToolStripMenuItem.Click += new System.EventHandler(this.InsertSplitToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBoxExpression);
            this.panel1.Controls.Add(this.cbPostFix);
            this.panel1.Controls.Add(this.lbFormat);
            this.panel1.Controls.Add(this.savingType);
            this.panel1.Controls.Add(this.lbXmlLang);
            this.panel1.Controls.Add(this.xmlLang);
            this.panel1.Controls.Add(this.cbChapterName);
            this.panel1.Controls.Add(this.cbAutoGenName);
            this.panel1.Controls.Add(this.cbShift);
            this.panel1.Controls.Add(this.btnLog);
            this.panel1.Controls.Add(this.lbShift);
            this.panel1.Controls.Add(this.numericUpDown1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // comboBoxExpression
            // 
            this.comboBoxExpression.FormattingEnabled = true;
            this.comboBoxExpression.Items.AddRange(new object[] {
            resources.GetString("comboBoxExpression.Items"),
            resources.GetString("comboBoxExpression.Items1")});
            resources.ApplyResources(this.comboBoxExpression, "comboBoxExpression");
            this.comboBoxExpression.Name = "comboBoxExpression";
            this.comboBoxExpression.SelectedIndexChanged += new System.EventHandler(this.comboBoxExpression_SelectedIndexChanged);
            this.comboBoxExpression.TextChanged += new System.EventHandler(this.comboBoxExpression_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsTips,
            this.tsProgressBar1,
            this.tsBtnExpand});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.SizingGrip = false;
            // 
            // tsTips
            // 
            this.tsTips.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsTips.Name = "tsTips";
            resources.ApplyResources(this.tsTips, "tsTips");
            this.tsTips.Spring = true;
            // 
            // tsProgressBar1
            // 
            this.tsProgressBar1.Name = "tsProgressBar1";
            resources.ApplyResources(this.tsProgressBar1, "tsProgressBar1");
            this.tsProgressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // tsBtnExpand
            // 
            this.tsBtnExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.tsBtnExpand, "tsBtnExpand");
            this.tsBtnExpand.Name = "tsBtnExpand";
            this.tsBtnExpand.ShowDropDownArrow = false;
            this.tsBtnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.cbRound);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.btnTrans);
            this.Controls.Add(this.lbPath);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(10)))), ((int)(((byte)(143)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.loadMenuStrip.ResumeLayout(false);
            this.deviationMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.combineMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.createZonestMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lbPath;
        private System.Windows.Forms.Button btnTrans;
        private System.Windows.Forms.CheckBox cbAutoGenName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox cbRound;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label lbShift;
        private System.Windows.Forms.CheckBox cbShift;
        private System.Windows.Forms.CheckBox cbChapterName;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ContextMenuStrip deviationMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsmAccuracy;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.ContextMenuStrip combineMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem combineToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox savingType;
        private System.Windows.Forms.Label lbFormat;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.ComboBox xmlLang;
        private System.Windows.Forms.Label lbXmlLang;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFrams;
        private System.Windows.Forms.DataGridViewTextBoxColumn cChapterName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTimeCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOrder;
        private System.Windows.Forms.ContextMenuStrip createZonestMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem creatZonesToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsTips;
        private System.Windows.Forms.ToolStripProgressBar tsProgressBar1;
        private System.Windows.Forms.ToolStripDropDownButton tsBtnExpand;
        private System.Windows.Forms.ToolStripMenuItem ShiftForwardToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbPostFix;
        private System.Windows.Forms.ContextMenuStrip loadMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem InsertSplitToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBoxExpression;
    }
}

