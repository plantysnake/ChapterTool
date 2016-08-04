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
using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ChapterTool.Forms
{
    public partial class FormPreview : Form
    {
        private Point _mainPos;
        private Form1 _mainWindow;

        public FormPreview(string text, Form1 mainWindow)
        {
            InitializeComponent();
            cTextBox1.Text = text;
            _mainWindow    = mainWindow;
            _mainPos       = mainWindow.Location;
            ScrollBarSet();
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
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
            _mainPos.X = _mainPos.X - Size.Width;
            Location   = _mainPos;
        }

        private void FormPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }


        private void FormPreview_Activated(object sender, EventArgs e)
        {
            //_mainWindow.Activate();
        }
    }
}
