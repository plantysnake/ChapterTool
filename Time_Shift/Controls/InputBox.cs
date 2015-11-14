using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChapterTool.Controls
{
    public class InputBox : Form
    {
        // Fields
        private Button btnCancel;
        private Button btnOk;
        private IContainer components = null;
        private Label lblMessage;
        private TextBox text;

        // Methods
        private InputBox()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
            base.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.OK;
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(InputBox));
            btnOk = new Button();
            btnCancel = new Button();
            text = new TextBox();
            lblMessage = new Label();
            base.SuspendLayout();
            btnOk.Location = new Point(0x107, 12);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(0x4b, 0x17);
            btnOk.TabIndex = 1;
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += new EventHandler(btnOk_Click);
            btnCancel.Location = new Point(0x107, 0x29);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(0x4b, 0x17);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += new EventHandler(btnCancel_Click);
            text.Location = new Point(12, 0x5d);
            text.Name = "text";
            text.Size = new Size(0x146, 0x15);
            text.TabIndex = 0;
            text.KeyPress += new KeyPressEventHandler(text_KeyPress);
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(12, 9);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0x23, 13);
            lblMessage.TabIndex = 3;
            lblMessage.Text = "label1";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(350, 0x7d);
            base.Controls.Add(lblMessage);
            base.Controls.Add(text);
            base.Controls.Add(btnCancel);
            base.Controls.Add(btnOk);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            //base.Icon = (Icon)manager.GetObject("$this.Icon");
            base.Name = "InputBox";
            base.StartPosition = FormStartPosition.CenterParent;
            Text = "InputBox";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        internal static string Show(string message, string title, string defaultText)
        {
            using (InputBox box = new InputBox())
            {
                box.lblMessage.Text = message;
                box.text.Text = defaultText;
                box.Text = title;
                if (box.ShowDialog() == DialogResult.OK)
                {
                    return box.text.Text;
                }
                return null;
            }
        }

        private void text_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnOk_Click(null, null);
            }
            else if (e.KeyChar == '\x001b')
            {
                btnCancel_Click(null, null);
            }
        }
    }

}
