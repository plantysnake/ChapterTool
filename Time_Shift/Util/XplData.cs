using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public static class XplData
    {
        public static List<ChapterInfo> GetStreams(string location)
        {
            List<ChapterInfo> pgcs = new List<ChapterInfo>();
            XDocument doc = XDocument.Load(location);
            XNamespace ns = "http://www.dvdforum.org/2005/HDDVDVideo/Playlist";
            foreach (XElement ts in doc.Element(ns + "Playlist").Elements(ns + "TitleSet"))
            {
                float timeBase = GetFps((string)ts.Attribute("timeBase"));
                float tickBase = GetFps((string)ts.Attribute("tickBase"));
                foreach (XElement title in ts.Elements(ns + "Title").Where(t => t.Element(ns + "ChapterList") != null))
                {
                    ChapterInfo pgc = new ChapterInfo
                    {
                        SourceName = location,
                        SourceType = "HD-DVD",
                        FramesPerSecond = 24D,
                        Chapters = new List<Chapter>()
                    };

                    int tickBaseDivisor = (int?)title.Attribute("tickBaseDivisor") ?? 1;
                    pgc.Duration = GetTimeSpan((string)title.Attribute("titleDuration"), timeBase, tickBase, tickBaseDivisor);
                    string titleName = Path.GetFileNameWithoutExtension(location);
                    if (title.Attribute("id") != null) titleName = (string)title.Attribute("id");
                    if (title.Attribute("displayName") != null) titleName = (string)title.Attribute("displayName");
                    pgc.Title = titleName;
                    foreach (XElement chapter in title.Element(ns + "ChapterList").Elements(ns + "Chapter"))
                    {
                        pgc.Chapters.Add(new Chapter
                        {
                            Name = (string)chapter.Attribute("displayName"),
                            Time = GetTimeSpan((string)chapter.Attribute("titleTimeBegin"), timeBase, tickBase, tickBaseDivisor)
                        });
                    }
                    //pgc.ChangeFps(24D / 1.001D);
                    pgcs.Add(pgc);
                }
            }
            pgcs = pgcs.OrderByDescending(p => p.Duration).ToList();
            return pgcs;
        }

        private static float GetFps(string fps)
        {
            if (string.IsNullOrEmpty(fps))
            {
                return 60.0f;
            }
            fps = fps.Replace("fps", string.Empty);
            return float.Parse(fps);
        }

        private static TimeSpan GetTimeSpan(string timeSpan, float timeBase, float tickBase, int tickBaseDivisor)
        {
            TimeSpan ts = TimeSpan.Parse(timeSpan.Substring(0, timeSpan.LastIndexOf(':')));
            ts = new TimeSpan((long)((ts.TotalSeconds / 60D) * timeBase) * TimeSpan.TicksPerSecond);

            //convert ticks to ticks timebase
            //NOTE: eac3to does not utilize tickBaseDivisor (they have bug)
            decimal convert = TimeSpan.TicksPerSecond / ((decimal)tickBase / tickBaseDivisor);
            decimal ticks = decimal.Parse(timeSpan.Substring(timeSpan.LastIndexOf(':') + 1)) * convert;
            return ts.Add(new TimeSpan((long)ticks));
        }

    }
}