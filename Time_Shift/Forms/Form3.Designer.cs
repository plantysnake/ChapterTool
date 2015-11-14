namespace ChapterTool.Forms
{
    partial class Form3
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
            this.back = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.textBack = new System.Windows.Forms.Button();
            this.overBack = new System.Windows.Forms.Button();
            this.downBack = new System.Windows.Forms.Button();
            this.bordBack = new System.Windows.Forms.Button();
            this.textFront = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // back
            //
            this.back.BackColor = System.Drawing.Color.WhiteSmoke;
            this.back.FlatAppearance.BorderSize = 0;
            this.back.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.back.Location = new System.Drawing.Point(12, 12);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(42, 42);
            this.back.TabIndex = 0;
            this.back.Text = " ";
            this.back.UseVisualStyleBackColor = false;
            this.back.Click += new System.EventHandler(this.back_Click);
            //
            // textBack
            //
            this.textBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.textBack.FlatAppearance.BorderSize = 0;
            this.textBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.textBack.Location = new System.Drawing.Point(60, 12);
            this.textBack.Name = "textBack";
            this.textBack.Size = new System.Drawing.Size(42, 42);
            this.textBack.TabIndex = 1;
            this.textBack.Text = " ";
            this.textBack.UseVisualStyleBackColor = false;
            this.textBack.Click += new System.EventHandler(this.textBack_Click);
            //
            // overBack
            //
            this.overBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(207)))), ((int)(((byte)(207)))));
            this.overBack.FlatAppearance.BorderSize = 0;
            this.overBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.overBack.Location = new System.Drawing.Point(108, 12);
            this.overBack.Name = "overBack";
            this.overBack.Size = new System.Drawing.Size(42, 42);
            this.overBack.TabIndex = 2;
            this.overBack.Text = " ";
            this.overBack.UseVisualStyleBackColor = false;
            this.overBack.Click += new System.EventHandler(this.overBack_Click);
            //
            // downBack
            //
            this.downBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(184)))), ((int)(((byte)(184)))));
            this.downBack.FlatAppearance.BorderSize = 0;
            this.downBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downBack.Location = new System.Drawing.Point(156, 12);
            this.downBack.Name = "downBack";
            this.downBack.Size = new System.Drawing.Size(42, 42);
            this.downBack.TabIndex = 3;
            this.downBack.Text = " ";
            this.downBack.UseVisualStyleBackColor = false;
            this.downBack.Click += new System.EventHandler(this.downBack_Click);
            //
            // bordBack
            //
            this.bordBack.BackColor = System.Drawing.SystemColors.Highlight;
            this.bordBack.FlatAppearance.BorderSize = 0;
            this.bordBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bordBack.Location = new System.Drawing.Point(300, 12);
            this.bordBack.Name = "bordBack";
            this.bordBack.Size = new System.Drawing.Size(42, 42);
            this.bordBack.TabIndex = 4;
            this.bordBack.Text = " ";
            this.bordBack.UseVisualStyleBackColor = false;
            this.bordBack.Click += new System.EventHandler(this.bordBack_Click);
            //
            // textFront
            //
            this.textFront.BackColor = System.Drawing.SystemColors.WindowText;
            this.textFront.FlatAppearance.BorderSize = 0;
            this.textFront.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.textFront.Location = new System.Drawing.Point(252, 12);
            this.textFront.Name = "textFront";
            this.textFront.Size = new System.Drawing.Size(42, 42);
            this.textFront.TabIndex = 5;
            this.textFront.Text = " ";
            this.textFront.UseVisualStyleBackColor = false;
            this.textFront.Click += new System.EventHandler(this.textFront_Click);
            //
            // Form3
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(354, 66);
            this.Controls.Add(this.textFront);
            this.Controls.Add(this.bordBack);
            this.Controls.Add(this.downBack);
            this.Controls.Add(this.overBack);
            this.Controls.Add(this.textBack);
            this.Controls.Add(this.back);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Color Setting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form3_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button back;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button textBack;
        private System.Windows.Forms.Button overBack;
        private System.Windows.Forms.Button downBack;
        private System.Windows.Forms.Button bordBack;
        private System.Windows.Forms.Button textFront;
    }
}