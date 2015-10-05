using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public class Chapter
    {
        public Chapter()
        {
            FramsInfo = string.Empty;
        }

        /// <summary>Chapter Number</summary>
        public int Number { get; set; }
        /// <summary>Chapter TimeStamp</summary>
        public TimeSpan Time { get; set; }
        /// <summary>Chapter Name</summary>
        public string Name { get; set; }
        public string FramsInfo { get; set; }
        public override string ToString()
        {
            return string.Format("{0} - {1}", Name, convertMethod.time2string(Time));
        }
    }
    public class ChapterInfo
    {
        public ChapterInfo()
        {
            Chapters = new List<Chapter>();
            offset = TimeSpan.Zero;
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
        public TimeSpan offset { get; set; }
        public override string ToString()
        {
            if (Chapters.Count != 1)
                return string.Format("{0} - {1}  -  {2}  -  [{3} Chapters]", Title, SourceName, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", System.Math.Floor(Duration.TotalHours), Duration.Minutes, Duration.Seconds, Duration.Milliseconds), Chapters.Count);
            else
                return string.Format("{0} - {1}  -  {2}  -  [{3} Chapter]", Title, SourceName, string.Format("{0:00}:{1:00}:{2:00}.{3:000}", System.Math.Floor(Duration.TotalHours), Duration.Minutes, Duration.Seconds, Duration.Milliseconds), Chapters.Count);
        }

        public void ChangeFps(double fps)
        {
            for (int i = 0; i < Chapters.Count; i++)
            {
                Chapter c = Chapters[i];
                double frames = c.Time.TotalSeconds * FramesPerSecond;
                Chapters[i] = new Chapter() { Name = c.Name, Time = new TimeSpan((long)Math.Round(frames / fps * TimeSpan.TicksPerSecond)) };
            }

            double totalFrames = Duration.TotalSeconds * FramesPerSecond;
            Duration = new TimeSpan((long)Math.Round((totalFrames / fps) * TimeSpan.TicksPerSecond));
            FramesPerSecond = fps;
        }

        public string getText(bool DONOTUSEName)
        {
            string lines = string.Empty;
            int i = 1;
            foreach (Chapter c in Chapters)
            {
                lines += ("CHAPTER" + c.Number.ToString("00") + "=" + convertMethod.time2string(c.Time) + Environment.NewLine);
                lines += ("CHAPTER" + c.Number.ToString("00") + "NAME=");
                if (DONOTUSEName)
                {
                    lines += "Chapter " + i.ToString("00") + Environment.NewLine;
                }
                else
                {
                    lines += c.Name + Environment.NewLine;
                }
                ++i;
            }
            return string.Concat(lines);
        }

        public void SaveText(string filename,bool DONOTUSEName)
        {
            List<string> lines = new List<string>();
            int i = 1;
            foreach (Chapter c in Chapters) 
            {
                lines.Add("CHAPTER" + i.ToString("00") + "=" + convertMethod.time2string(c.Time));
                if (DONOTUSEName)
                {
                    lines.Add("Chapter " + i.ToString("00"));
                }
                else
                {
                    lines.Add("CHAPTER" + i.ToString("00") + "NAME=" + c.Name);
                }
                ++i;
            }
            File.WriteAllLines(filename, lines.ToArray(),Encoding.UTF8);
        }

        public void SaveQpfile(string filename)
        {
            List<string> lines = new List<string>();
            foreach (Chapter c in Chapters)
                lines.Add(c.FramsInfo.ToString());
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveCelltimes(string filename)
        {
            List<string> lines = new List<string>();
            foreach (Chapter c in Chapters)
                lines.Add(((long)Math.Round(c.Time.TotalSeconds * FramesPerSecond)).ToString());
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveTsmuxerMeta(string filename)
        {
            string text = "--custom-" + Environment.NewLine + "chapters=";
            foreach (Chapter c in Chapters)
                text += c.Time.ToString() + ";";
            text = text.Substring(0, text.Length - 1);
            File.WriteAllText(filename, text);
        }

        public void SaveTimecodes(string filename)
        {
            List<string> lines = new List<string>();
            foreach (Chapter c in Chapters)
                lines.Add(c.Time.ToString());
            File.WriteAllLines(filename, lines.ToArray());
        }

        public void SaveXml(string filename)
        {
            Random rndb = new Random();
            XmlTextWriter xmlchap = new XmlTextWriter(filename, Encoding.UTF8);
            xmlchap.Formatting = Formatting.Indented;
            xmlchap.WriteStartDocument();
            xmlchap.WriteComment("<!DOCTYPE Tags SYSTEM " + "\"" + "matroskatags.dtd" + "\"" + ">");
            xmlchap.WriteStartElement("Chapters");
            xmlchap.WriteStartElement("EditionEntry");
            xmlchap.WriteElementString("EditionFlagHidden", "0");
            xmlchap.WriteElementString("EditionFlagDefault", "0");
            xmlchap.WriteElementString("EditionUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
            foreach (Chapter c in Chapters)
            {
                xmlchap.WriteStartElement("ChapterAtom");
                xmlchap.WriteStartElement("ChapterDisplay");
                xmlchap.WriteElementString("ChapterString", c.Name);
                xmlchap.WriteElementString("ChapterLanguage", string.IsNullOrEmpty(LangCode) ? "eng" : LangCode);
                xmlchap.WriteEndElement();
                xmlchap.WriteElementString("ChapterUID", Convert.ToString(rndb.Next(1, int.MaxValue)));
                xmlchap.WriteElementString("ChapterTimeStart", convertMethod.time2string(c.Time) + "0000");
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
