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
            this.cTextBox1 = new ChapterTool.cTextBox();
            this.SuspendLayout();
            // 
            // cTextBox1
            // 
            this.cTextBox1.Location = new System.Drawing.Point(12, 12);
            this.cTextBox1.Multiline = true;
            this.cTextBox1.Name = "cTextBox1";
            this.cTextBox1.OthercTextBox = null;
            this.cTextBox1.Size = new System.Drawing.Size(260, 237);
            this.cTextBox1.TabIndex = 0;
            // 
            // FormPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.cTextBox1);
            this.Name = "FormPreview";
            this.Text = "FormPreview";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private cTextBox cTextBox1;
    }
}