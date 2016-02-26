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
using System.Windows.Forms;

namespace ChapterTool.Util
{
    public class Chapter
    {
        /// <summary>Chapter Number</summary>
        public int Number       { get; set; }
        /// <summary>Chapter TimeStamp</summary>
        public TimeSpan Time    { get; set; }
        /// <summary>Chapter Name</summary>
        public string Name      { get; set; }
        /// <summary>Fram Count</summary>
        public string FramsInfo { get; set; } = string.Empty;
        public override string ToString() => $"{Name} - {Time.Time2String()}";

        public Chapter() { }

        public Chapter(string name, TimeSpan time, int number)
        {
            Number = number;
            Time   = time;
            Name   = name;
        }

        public DataGridViewRow ToRow(ChapterInfo info, bool autoGenName)
        {
            var row = new DataGridViewRow
            {
                Tag = this,
                DefaultCellStyle =
                {
                    BackColor = (Number + 1)%2 == 0 ? Color.FromArgb(0x92, 0xAA, 0xF3)
                                                    : Color.FromArgb(0xF3, 0xF7, 0xF7)
                }
            };
            row.Cells.Add(new DataGridViewTextBoxCell { Value = $"{Number:D2}" });
            row.Cells.Add(new DataGridViewTextBoxCell { Value = this.Time2String(info) });
            row.Cells.Add(new DataGridViewTextBoxCell { Value = autoGenName ? $"Chapter {row.Index + 1:D2}" : Name });
            row.Cells.Add(new DataGridViewTextBoxCell { Value = FramsInfo });
            return row;
        }


        public int GetAccuracy(decimal fps, decimal accuracy)
        {
            var frams   = (decimal)Time.TotalMilliseconds * fps / 1000M;
            var rounded = Math.Round(frams, MidpointRounding.AwayFromZero);
            return Math.Abs(frams - rounded) < accuracy ? 1 : 0;
        }
    }
}
