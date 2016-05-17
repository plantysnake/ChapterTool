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
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public class ChapterInfo
    {
        /// <summary>
        /// The title of Chapter
        /// </summary>
        public string Title           { get; set; }
        /// <summary>
        /// Corresponding Video file
        /// </summary>
        public string SourceName      { get; set; }
        public string SourceType      { get; set; }
        public double FramesPerSecond { get; set; }
        public TimeSpan Duration      { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public TimeSpan Offset        { get; set; } = TimeSpan.Zero;
        public bool Mul1K1            { get; set; }
        public Type TagType { get; set; }
        public object Tag
        {
            get { return _tag; }
            set
            {
                if (value == null)
                    return;
                _tag = value;
            }
        }
        private object _tag;

        public override string ToString() => $"{Title} - {SourceType} - {Duration.Time2String()} - [{Chapters.Count} Chapters]";

        private readonly Color EVEN_COLOR = Color.FromArgb(0xFF, 0x92, 0xAA, 0xF3);
        private readonly Color ODD_COLOR  = Color.FromArgb(0xFF, 0xF3, 0xF7, 0xF7);

        public DataGridViewRow GetRow(int index, bool autoGenName)
        {
            var row = new DataGridViewRow
            {
                Tag = Chapters[index],  //绑定对象，以便删除行时可以得知对于的 Chapter
                DefaultCellStyle =      //设定背景色交替
                {
                    BackColor = (Chapters[index].Number-1)%2 == 0 ? EVEN_COLOR : ODD_COLOR
                }
            };
            row.Cells.Add(new DataGridViewTextBoxCell {Value = $"{Chapters[index].Number:D2}"});
            row.Cells.Add(new DataGridViewTextBoxCell {Value = Time2String(Chapters[index]) });
            row.Cells.Add(new DataGridViewTextBoxCell {Value = autoGenName ? ChapterName.Get(row.Index + 1) : Chapters[index].Name});
            row.Cells.Add(new DataGridViewTextBoxCell {Value = Chapters[index].FramsInfo});
            return row;
        }

        /// <summary>
        /// 在无行数变动时直接修改各行的数据
        /// 提高刷新效率
        /// </summary>
        /// <param name="row">要更改的行</param>
        /// <param name="autoGenName">是否使用自动生成的章节名</param>
        public void EditRow(DataGridViewRow row, bool autoGenName)
        {
            var item = Chapters[row.Index];
            row.Tag  = item;
            row.DefaultCellStyle.BackColor = row.Index%2 == 0 ? EVEN_COLOR : ODD_COLOR;
            row.Cells[0].Value = $"{item.Number:D2}";
            row.Cells[1].Value = item.Time2String(this);
            row.Cells[2].Value = autoGenName ? ChapterName.Get(row.Index + 1) : item.Name;
            row.Cells[3].Value = item.FramsInfo;
        }

        /// <summary>
        /// 将分开多段的 ifo 章节合并为一个章节
        /// </summary>
        /// <param name="source">解析获得的分段章节</param>
        /// <returns></returns>
        public static ChapterInfo CombineChapter(List<ChapterInfo> source)
        {
            var fullChapter = new ChapterInfo
            {
                Title           = "FULL Chapter",
                SourceType      = "DVD",
                FramesPerSecond = source.First().FramesPerSecond
            };
            var duration = TimeSpan.Zero;
            var name     = new ChapterName();
            foreach (var chapterClip in source)
            {
                foreach (var item in chapterClip.Chapters)
                {
                    fullChapter.Chapters.Add(new Chapter
                    {
                        Time   = duration + item.Time,
                        Number = name.Index,
                        Name   = name.Get()
                    });
                }
                duration += chapterClip.Duration;//每次加上当前段的总时长作为下一段位移的基准
            }
            fullChapter.Duration = duration;
            return fullChapter;
        }

        private string Time2String(Chapter item)
        {
            return item.Time2String(this);
        }

        public void ChangeFps(double fps)
        {
            for (var i = 0; i < Chapters.Count; i++)
            {
                Chapter c = Chapters[i];
                double frames = c.Time.TotalSeconds * FramesPerSecond;
                Chapters[i] = new Chapter { Name = c.Name, Time = new TimeSpan((long)Math.Round(frames / fps * TimeSpan.TicksPerSecond)) };
            }
            double totalFrames = Duration.TotalSeconds * FramesPerSecond;
            Duration = new TimeSpan((long)Math.Round(totalFrames / fps * TimeSpan.TicksPerSecond));
            FramesPerSecond = fps;
        }

        #region updataInfo

        /// <summary>
        /// 以新的时间基准更新剩余章节
        /// </summary>
        /// <param name="shift">剩余章节的首个章节点的时间</param>
        public void UpdataInfo(TimeSpan shift)
        {
            Chapters.ForEach(item => item.Time -= shift);
        }

        /// <summary>
        /// 根据输入的数值向后位移章节序号
        /// </summary>
        /// <param name="shift">位移量</param>
        public void UpdataInfo(int shift)
        {
            int index = 0;
            Chapters.ForEach(item => item.Number = ++index + shift);
        }

        /// <summary>
        /// 根据给定的章节名模板更新章节
        /// </summary>
        /// <param name="chapterNameTemplate"></param>
        public void UpdataInfo(string chapterNameTemplate)
        {
            if (string.IsNullOrWhiteSpace(chapterNameTemplate)) return;
            var cn = chapterNameTemplate.Trim(' ', '\r', '\n').Split('\n').ToList().GetEnumerator();//移除首尾多余空行
            Chapters.ForEach(item => item.Name = cn.MoveNext() ? cn.Current : item.Name.Trim('\r'));//确保无多余换行符
            cn.Dispose();
        }

        #endregion

        /// <summary>
        /// 生成 OGM 样式章节
        /// </summary>
        /// <param name="notUseName">不使用章节名</param>
        /// <returns></returns>
        public string GetText(bool notUseName)
        {
            var lines = new StringBuilder();
            var name  = ChapterName.GetChapterName("Chapter");
            foreach (var item in Chapters)
            {
                lines.Append($"CHAPTER{item.Number:D2}={Time2String(item)}{Environment.NewLine}");
                lines.Append($"CHAPTER{item.Number:D2}NAME=");
                lines.Append(notUseName ? name() : item.Name);
                lines.Append(Environment.NewLine);
            }
            return lines.ToString();
        }

        public void SaveText(string filename, bool notUseName) => File.WriteAllText(filename, GetText(notUseName), Encoding.UTF8);

        public void SaveQpfile(string filename) => File.WriteAllLines(filename, Chapters.Select(c => c.FramsInfo.ToString().Replace("*", "I").Replace("K", "I")).ToArray());

        public void SaveCelltimes(string filename) => File.WriteAllLines(filename, Chapters.Select(c => ((long) Math.Round(c.Time.TotalSeconds*FramesPerSecond)).ToString()).ToArray());

        public void SaveTsmuxerMeta(string filename)
        {
            string text = $"--custom-{Environment.NewLine}chapters=";
            text = Chapters.Aggregate(text, (current, chapter) => current + Time2String(chapter) + ";");
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename) => File.WriteAllLines(filename, Chapters.Select(Time2String).ToArray());

        public void SaveXml(string filename ,string lang, bool notUseName)
        {
            if (string.IsNullOrWhiteSpace(lang)) lang = "und";
            Random rndb           = new Random();
            XmlTextWriter xmlchap = new XmlTextWriter(filename, Encoding.UTF8) {Formatting = Formatting.Indented};
            xmlchap.WriteStartDocument();
            xmlchap.WriteComment("<!DOCTYPE Tags SYSTEM \"matroskatags.dtd\">");
            xmlchap.WriteStartElement("Chapters");
              xmlchap.WriteStartElement("EditionEntry");
                xmlchap.WriteElementString("EditionFlagHidden", "0");
                xmlchap.WriteElementString("EditionFlagDefault", "0");
                xmlchap.WriteElementString("EditionUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
                var name = ChapterName.GetChapterName("Chapter");
                foreach (var item in Chapters)
                {
                    xmlchap.WriteStartElement("ChapterAtom");
                      xmlchap.WriteStartElement("ChapterDisplay");
                        xmlchap.WriteElementString("ChapterString", notUseName ? name() : item.Name);
                        xmlchap.WriteElementString("ChapterLanguage", lang);
                      xmlchap.WriteEndElement();
                    xmlchap.WriteElementString("ChapterUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
                    xmlchap.WriteElementString("ChapterTimeStart", Time2String(item) + "000");
                    xmlchap.WriteElementString("ChapterFlagHidden", "0");
                    xmlchap.WriteElementString("ChapterFlagEnabled", "1");
                    xmlchap.WriteEndElement();
                }
              xmlchap.WriteEndElement();
            xmlchap.WriteEndElement();
            xmlchap.Flush();
            xmlchap.Close();
        }

        public void SaveCue(string sourceFileName, string fileName, bool notUseName)
        {
            StringBuilder cueBuilder = new StringBuilder();
            cueBuilder.AppendLine("REM Generate By ChapterTool");
            cueBuilder.AppendLine($"TITLE \"{Title}\"");

            cueBuilder.AppendLine($"FILE \"{sourceFileName}\" WAVE");
            int index = 0;
            var name = ChapterName.GetChapterName("Chapter");
            foreach (var chapter in Chapters)
            {
                cueBuilder.AppendLine($"  TRACK {++index:D2} AUDIO");
                cueBuilder.AppendLine($"    TITLE \"{(notUseName ? name(): chapter.Name)}\"");
                cueBuilder.AppendLine($"    INDEX 01 {chapter.Time.ToCueTimeStamp()}");
            }
            File.WriteAllText(fileName, cueBuilder.ToString());
        }
    }
}
