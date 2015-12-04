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
        public ChapterInfo()
        {
            Chapters = new List<Chapter>();
            Offset   = TimeSpan.Zero;
        }
        public string Title { get; set; }
        public string LangCode { get; set; }
        public string SourceName { get; set; }
        public int TitleNumber { get; set; }
        public string SourceType { get; set; }
        public string SourceHash { get; set; }
        public double FramesPerSecond { get; set; }
        public TimeSpan Duration { get; set; }
        public List<Chapter> Chapters { get; set; }
        public TimeSpan Offset { get; set; }
        public override string ToString() => $"{Title} - {SourceName}  -  {ConvertMethod.Time2String(Duration)}  -  [{Chapters.Count} Chapter]";

        public void ChangeFps(double fps)
        {
            for (var i = 0; i < Chapters.Count; i++)
            {
                Chapter c = Chapters[i];
                double frames = c.Time.TotalSeconds * FramesPerSecond;
                Chapters[i] = new Chapter() { Name = c.Name, Time = new TimeSpan((long)Math.Round(frames / fps * TimeSpan.TicksPerSecond)) };
            }

            double totalFrames = Duration.TotalSeconds * FramesPerSecond;
            Duration = new TimeSpan((long)Math.Round((totalFrames / fps) * TimeSpan.TicksPerSecond));
            FramesPerSecond = fps;
        }

        public string GetText(bool donotuseName)
        {
            StringBuilder lines = new StringBuilder();
            int i = 1;
            foreach (Chapter c in Chapters)
            {
                lines.Append($"CHAPTER{c.Number:D2}={ConvertMethod.Time2String(c.Time)}{Environment.NewLine}");
                lines.Append($"CHAPTER{c.Number:D2}NAME=");
                lines.Append(donotuseName ? $"Chapter {i++:D2}" : c.Name);
                lines.Append(Environment.NewLine);
            }
            return lines.ToString();
        }

        public void SaveText(string filename,bool donotuseName)
        {
            StringBuilder lines = new StringBuilder();
            int i = 1;
            foreach (Chapter c in Chapters)
            {
                lines.Append($"CHAPTER{c.Number:D2}={ConvertMethod.Time2String(c.Time)}{Environment.NewLine}");
                lines.Append($"CHAPTER{c.Number:D2}NAME=");
                lines.Append(donotuseName ? $"Chapter {i++:D2}" : c.Name);
                lines.Append(Environment.NewLine);
            }
            File.WriteAllText(filename, lines.ToString(), Encoding.UTF8);
        }

        public void SaveQpfile(string filename) => File.WriteAllLines(filename, Chapters.Select(c => c.FramsInfo.ToString()).ToArray());

        public void SaveCelltimes(string filename) => File.WriteAllLines(filename, Chapters.Select(c => ((long) Math.Round(c.Time.TotalSeconds*FramesPerSecond)).ToString()).ToArray());

        public void SaveTsmuxerMeta(string filename)
        {
            string text = $"--custom-{Environment.NewLine}chapters=";
            text = Chapters.Aggregate(text, (current, c) => current + (c.Time.ToString() + ";"));
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename) => File.WriteAllLines(filename, Chapters.Select(c => c.Time.ToString()).ToArray());

        public void SaveXml(string filename,string lang, bool donotuseName)
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
            foreach (Chapter c in Chapters)
            {
                xmlchap.WriteStartElement("ChapterAtom");
                xmlchap.WriteStartElement("ChapterDisplay");
                xmlchap.WriteElementString("ChapterString", donotuseName ? $"Chapter {i++:D2}" : c.Name);
                xmlchap.WriteElementString("ChapterLanguage", lang);
                xmlchap.WriteEndElement();
                xmlchap.WriteElementString("ChapterUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
                xmlchap.WriteElementString("ChapterTimeStart", ConvertMethod.Time2String(c.Time) + "0000");
                xmlchap.WriteElementString("ChapterFlagHidden", "0");
                xmlchap.WriteElementString("ChapterFlagEnabled", "1");
                xmlchap.WriteEndElement();
            }
            xmlchap.WriteEndElement();
            xmlchap.WriteEndElement();
            xmlchap.Flush();
            xmlchap.Close();
        }
    }
}
