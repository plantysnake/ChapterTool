using System;
using System.Linq;
using System.Text.RegularExpressions;
using static ChapterTool.Util.CTLogger;
using static ChapterTool.Util.ConvertMethod;

namespace ChapterTool.Util
{
    public static class OgmData
    {
        private static readonly Regex RLineOne = new Regex(@"CHAPTER\d+=\d+:\d+:\d+\.\d+");
        private static readonly Regex RLineTwo = new Regex(@"CHAPTER\d+NAME=(?<chapterName>.*)");

        public static ChapterInfo GetChapterInfo(string text, bool autoGenName)
        {
            int index = 0;
            var info = new ChapterInfo { SourceType = "OGM", Tag = text, TagType = text.GetType() };
            var ogmData = text.Trim(' ', '\r', '\n').Split('\n').SkipWhile(string.IsNullOrWhiteSpace).GetEnumerator();
            if (!ogmData.MoveNext()) return info;
            TimeSpan iniTime = OffsetCal(ogmData.Current);
            do
            {
                string buffer1 = ogmData.Current;
                ogmData.MoveNext();
                string buffer2 = ogmData.Current;
                if (string.IsNullOrWhiteSpace(buffer1) || string.IsNullOrWhiteSpace(buffer2))
                {
                    Log($"interrupt at '{buffer1}'  '{buffer2}'");
                    break;
                }
                if (RLineOne.IsMatch(buffer1) && RLineTwo.IsMatch(buffer2))
                {
                    info.Chapters.Add(WriteToChapterInfo(buffer1, buffer2, ++index, iniTime, autoGenName));
                    continue;
                }
                throw new FormatException($"invalid format: \n'{buffer1}' \n'{buffer2}' ");
            } while (ogmData.MoveNext());
            if (info.Chapters.Count > 1)
            {
                info.Duration = info.Chapters.Last().Time;
            }
            ogmData.Dispose();
            return info;
        }

        private static TimeSpan OffsetCal(string line)
        {
            if (RLineOne.IsMatch(line))
            {
                return RTimeFormat.Match(line).Value.ToTimeSpan();
            }
            throw new Exception($"ERROR: {line} <-该行与时间行格式不匹配");
        }

        private static Chapter WriteToChapterInfo(string line, string line2, int order, TimeSpan iniTime, bool notUseName)
        {
            Chapter temp = new Chapter { Number = order, Time = TimeSpan.Zero };
            if (!RLineOne.IsMatch(line) || !RLineTwo.IsMatch(line2)) return temp;
            temp.Name = notUseName ? $"Chapter {order:D2}"
                : RLineTwo.Match(line2).Groups["chapterName"].Value.Trim('\r');
            temp.Time = RTimeFormat.Match(line).Value.ToTimeSpan() - iniTime;
            return temp;
        }
    }
}
