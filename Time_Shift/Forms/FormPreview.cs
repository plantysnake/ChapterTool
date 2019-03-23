// ****************************************************************************
//
// Copyright (C) 2014-2016 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************
namespace ChapterTool.Forms
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public partial class FormPreview : Form
    {
        private readonly Form1 _mainForm;

        internal const int WS_EX_TOPMOST = 8;

        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                if (TopMost) p.ExStyle |= WS_EX_TOPMOST;
                return p;
            }
        }

        public FormPreview(Form1 mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _mainForm.Move += Form1_Move;
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void ScrollBarSet()
        {
            if (cTextBox1.Lines.Length > 20 && cTextBox1.Lines.Max(item => item.Length) > 40)
            {
                cTextBox1.ScrollBars = ScrollBars.Both;
            }
            else if (cTextBox1.Lines.Length > 20)
            {
                cTextBox1.ScrollBars = ScrollBars.Vertical;
            }
        }

        public void UpdateText(string text)
        {
            cTextBox1.Text = text;
            ScrollBarSet();
        }

        private void cTextBox1_DoubleClick(object sender, EventArgs e) => Close();

        private void FormPreview_Load(object sender, EventArgs e)
        {
            Form1_Move(this, null);
        }

        private void FormPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            Location = new Point(_mainForm.Location.X - Width, _mainForm.Location.Y);
        }
    }
}
