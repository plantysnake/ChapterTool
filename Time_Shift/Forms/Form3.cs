// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
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
using System.Drawing;
using ChapterTool.Util;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChapterTool.Forms
{
    public partial class Form3 : Form
    {
        private readonly Form1 _mainWindow;
        private readonly List<Color> _currentSetting;
        public Form3(Form1 mainWindow)
        {
            InitializeComponent();
            MaximizeBox     = false;
            _mainWindow     = mainWindow;
            _currentSetting = mainWindow.CurrentColor;
            SetDefault();
            AddToolTip();
        }

        private void AddToolTip()
        {
            back.MouseEnter      += (sender, args) => toolTip1.Show(@"窗体背景色", (IWin32Window)sender);
            textBack.MouseEnter  += (sender, args) => toolTip1.Show(@"文字背景色", (IWin32Window)sender);
            overBack.MouseEnter  += (sender, args) => toolTip1.Show(@"按键默认色", (IWin32Window)sender);
            downBack.MouseEnter  += (sender, args) => toolTip1.Show(@"按键激活色", (IWin32Window)sender);
            textFront.MouseEnter += (sender, args) => toolTip1.Show(@"文字前景色", (IWin32Window)sender);
            bordBack.MouseEnter  += (sender, args) => toolTip1.Show(@"按键边框色", (IWin32Window)sender);

            back.MouseLeave      += (sender, args) => toolTip1.RemoveAll();
            textBack.MouseLeave  += (sender, args) => toolTip1.RemoveAll();
            overBack.MouseLeave  += (sender, args) => toolTip1.RemoveAll();
            downBack.MouseLeave  += (sender, args) => toolTip1.RemoveAll();
            textFront.MouseLeave += (sender, args) => toolTip1.RemoveAll();
            bordBack.MouseLeave  += (sender, args) => toolTip1.RemoveAll();
        }

        private void SetDefault()
        {
            back.BackColor      = _currentSetting[0];
            textBack.BackColor  = _currentSetting[1];
            overBack.BackColor  = _currentSetting[2];
            downBack.BackColor  = _currentSetting[3];
            bordBack.BackColor  = _currentSetting[4];
            textFront.BackColor = _currentSetting[5];
        }

        private void back_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.BackChange = back.BackColor = colorDialog1.Color;
            }
        }
        private void textBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.TextBack = textBack.BackColor = colorDialog1.Color;
            }
        }
        private void overBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.MouseOverColor = overBack.BackColor = colorDialog1.Color;
            }
        }
        private void downBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.MouseDownColor = downBack.BackColor = colorDialog1.Color;
            }
        }
        private void bordBack_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.BordBackColor = bordBack.BackColor = colorDialog1.Color;
            }
        }
        private void textFront_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _mainWindow.TextFrontColor = textFront.BackColor = colorDialog1.Color;
            }
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mainWindow.CurrentColor.SaveColor();
            e.Cancel = true;
            Hide();
        }
    }
}
