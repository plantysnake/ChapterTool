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
        public string Title           { get; set; }
        public string LangCode        { get; set; }
        public string SourceName      { get; set; }
        public int TitleNumber        { get; set; }
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

        public DataGridViewRow GetRow(int index, bool autoGenName)
        {
            var row = new DataGridViewRow
            {
                Tag = Chapters[index],  //绑定对象，以便修改信息时可以得知对于的 Chapter
                DefaultCellStyle =      //设定背景色交替
                {
                    BackColor = (Chapters[index].Number + 1)%2 == 0
                        ? Color.FromArgb(0x92, 0xAA, 0xF3)
                        : Color.FromArgb(0xF3, 0xF7, 0xF7)
                }
            };
            row.Cells.Add(new DataGridViewTextBoxCell {Value = $"{Chapters[index].Number:D2}"});
            row.Cells.Add(new DataGridViewTextBoxCell {Value = Chapters[index].Time2String(Offset, Mul1K1)});
            row.Cells.Add(new DataGridViewTextBoxCell {Value = autoGenName ? $"Chapter {row.Index + 1:D2}" : Chapters[index].Name});
            row.Cells.Add(new DataGridViewTextBoxCell {Value = Chapters[index].FramsInfo});
            return row;
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
                Title = "FULL Chapter",
                SourceType = "DVD",
                FramesPerSecond = source.First().FramesPerSecond
            };
            TimeSpan duration = TimeSpan.Zero;
            int index = 0;
            source.ForEach(chapterClip =>
            {
                chapterClip.Chapters.ForEach(item =>
                    fullChapter.Chapters.Add(new Chapter
                    {
                        Time = duration + item.Time,
                        Number = ++index,
                        Name = $"Chapter {index:D2}"
                    }));
                duration += chapterClip.Duration;//每次加上当前段的总时长作为下一段位移的基准
            });
            fullChapter.Duration = duration;
            return fullChapter;
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
            StringBuilder lines = new StringBuilder();
            int i = 1;
            Chapters.ForEach(item =>
            {
                lines.Append($"CHAPTER{item.Number:D2}={item.Time2String(Offset, Mul1K1)}{Environment.NewLine}");
                lines.Append($"CHAPTER{item.Number:D2}NAME=");
                lines.Append(notUseName ? $"Chapter {i++:D2}" : item.Name);
                lines.Append(Environment.NewLine);
            });
            return lines.ToString();
        }

        public void SaveText(string filename, bool notUseName)
        {
            File.WriteAllText(filename, GetText(notUseName), Encoding.UTF8);
        }

        public void SaveQpfile(string filename) => File.WriteAllLines(filename, Chapters.Select(c => c.FramsInfo.ToString().Replace("*", "I -1").Replace("K", "I -1")).ToArray());

        public void SaveCelltimes(string filename) => File.WriteAllLines(filename, Chapters.Select(c => ((long) Math.Round(c.Time.TotalSeconds*FramesPerSecond)).ToString()).ToArray());

        public void SaveTsmuxerMeta(string filename)
        {
            string text = $"--custom-{Environment.NewLine}chapters=";
            text = Chapters.Aggregate(text, (current, chapter) => current + chapter.Time2String(Offset, Mul1K1) + ";");
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename) => File.WriteAllLines(filename, Chapters.Select(item => item.Time2String(Offset, Mul1K1)).ToArray());

        public void SaveXml(string filename,string lang, bool notUseName)
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
                int i = 1;
                Chapters.ForEach(item =>
                {
                    xmlchap.WriteStartElement("ChapterAtom");
                      xmlchap.WriteStartElement("ChapterDisplay");
                        xmlchap.WriteElementString("ChapterString", notUseName ? $"Chapter {i++:D2}" : item.Name);
                        xmlchap.WriteElementString("ChapterLanguage", lang);
                      xmlchap.WriteEndElement();
                    xmlchap.WriteElementString("ChapterUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
                    xmlchap.WriteElementString("ChapterTimeStart", item.Time2String(Offset, Mul1K1) + "0000");
                    xmlchap.WriteElementString("ChapterFlagHidden", "0");
                    xmlchap.WriteElementString("ChapterFlagEnabled", "1");
                    xmlchap.WriteEndElement();
                });
              xmlchap.WriteEndElement();
            xmlchap.WriteEndElement();
            xmlchap.Flush();
            xmlchap.Close();
        }
    }
}
