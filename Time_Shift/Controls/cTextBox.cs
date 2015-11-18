// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
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
using System.Windows.Forms;

namespace ChapterTool.Controls
{
    // ReSharper disable once InconsistentNaming
    public class cTextBox : TextBox
    {
        // Methods
        public cTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                if (e.Control && (e.KeyCode == Keys.A))
                {
                    SelectAll();
                }
                else if (e.Control && (e.KeyCode == Keys.C))
                {
                    Clipboard.SetText(SelectedText, TextDataFormat.UnicodeText);
                }
                else if (e.Alt && (e.KeyCode == Keys.A))
                {
                    int totalLine = GetLineFromCharIndex(Text.Length);
                    int charIndex = GetFirstCharIndexFromLine(totalLine / 2);
                    Select(charIndex, Text.Length);
                }
                e.Handled = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }



}
