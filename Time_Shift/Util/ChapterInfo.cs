using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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
        public override string ToString() => $"{Title} - {SourceName}  -  {ConvertMethod.Time2String(Duration)}  -  [{Chapters.Count} Chapter]";

        public static Chapter WriteToChapterInfo(string line, string line2, int order, TimeSpan iniTime, bool notUseName)
        {
            Chapter temp = new Chapter { Number = order, Time = TimeSpan.Zero };
            if (!ConvertMethod.RLineOne.IsMatch(line) || !ConvertMethod.RLineTwo.IsMatch(line2)) return temp;
            temp.Name = notUseName ? $"Chapter {order:D2}"
                : ConvertMethod.RLineTwo.Match(line2).Groups["chapterName"].Value.Trim('\r');
            temp.Time = ConvertMethod.String2Time(ConvertMethod.RTimeFormat.Match(line).Value) - iniTime;
            return temp;
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

        public string GetText(bool donotuseName)
        {
            StringBuilder lines = new StringBuilder();
            int i = 1;
            Chapters.ForEach(item =>
            {
                lines.Append($"CHAPTER{item.Number:D2}={ConvertMethod.Time2String(item.Time)}{Environment.NewLine}");
                lines.Append($"CHAPTER{item.Number:D2}NAME=");
                lines.Append(donotuseName ? $"Chapter {i++:D2}" : item.Name);
                lines.Append(Environment.NewLine);
            });
            return lines.ToString();
        }

        public void SaveText(string filename, bool notUseName)
        {
            StringBuilder lines = new StringBuilder();
            int i = 1;
            Chapters.ForEach(item =>
            {
                lines.Append($"CHAPTER{item.Number:D2}={ConvertMethod.Time2String(item, Offset, Mul1K1)}{Environment.NewLine}");
                lines.Append($"CHAPTER{item.Number:D2}NAME=");
                lines.Append(notUseName ? $"Chapter {i++:D2}" : item.Name);
                lines.Append(Environment.NewLine);
            });
            File.WriteAllText(filename, lines.ToString(), Encoding.UTF8);
        }

        public void SaveQpfile(string filename) => File.WriteAllLines(filename, Chapters.Select(c => c.FramsInfo.ToString().Replace("*", "I -1").Replace("K", "I -1")).ToArray());

        public void SaveCelltimes(string filename) => File.WriteAllLines(filename, Chapters.Select(c => ((long) Math.Round(c.Time.TotalSeconds*FramesPerSecond)).ToString()).ToArray());

        public void SaveTsmuxerMeta(string filename)
        {
            string text = $"--custom-{Environment.NewLine}chapters=";
            text = Chapters.Aggregate(text, (current, chapter) => current + ConvertMethod.Time2String(chapter, Offset, Mul1K1) + ";");
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename) => File.WriteAllLines(filename, Chapters.Select(item => ConvertMethod.Time2String(item, Offset, Mul1K1)).ToArray());

        public void SaveXml(string filename,string lang, bool notUseName)
        {
            if (string.IsNullOrEmpty(lang)) { lang = "und"; }
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
                xmlchap.WriteElementString("ChapterTimeStart", ConvertMethod.Time2String(item, Offset, Mul1K1) + "0000");
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
