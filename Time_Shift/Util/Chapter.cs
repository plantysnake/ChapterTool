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

        public DataGridViewRow ToRow(DataGridView grid, ChapterInfo info, bool autoGenName)
        {
            var row = new DataGridViewRow
            {
                Tag = this,
                DefaultCellStyle =
                {
                    BackColor = (Number + 1)%2 == 0
                        ? Color.FromArgb(0x92, 0xAA, 0xF3)
                        : Color.FromArgb(0xF3, 0xF7, 0xF7)
                }
            };
            row.CreateCells(grid);
            row.Cells[0].Value = $"{Number:D2}";
            row.Cells[1].Value = this.Time2String(info.Offset, info.Mul1K1);
            row.Cells[2].Value = autoGenName ? $"Chapter {row.Index + 1:D2}" : Name;
            row.Cells[3].Value = FramsInfo;
            return row;
        }
    }
}
