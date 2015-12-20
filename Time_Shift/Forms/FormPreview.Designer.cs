namespace ChapterTool.Forms
{
    partial class FormPreview
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
            this.cTextBox1 = new ChapterTool.Controls.cTextBox();
            this.SuspendLayout();
            // 
            // cTextBox1
            // 
            this.cTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cTextBox1.Location = new System.Drawing.Point(0, 0);
            this.cTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.cTextBox1.Multiline = true;
            this.cTextBox1.Name = "cTextBox1";
            this.cTextBox1.ReadOnly = true;
            this.cTextBox1.Size = new System.Drawing.Size(230, 300);
            this.cTextBox1.TabIndex = 0;
            this.cTextBox1.TabStop = false;
            this.cTextBox1.WordWrap = false;
            this.cTextBox1.DoubleClick += new System.EventHandler(this.cTextBox1_DoubleClick);
            // 
            // FormPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 300);
            this.Controls.Add(this.cTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPreview";
            this.Text = "Preview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPreview_FormClosing);
            this.Load += new System.EventHandler(this.FormPreview_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ChapterTool.Controls.cTextBox cTextBox1;
    }
}